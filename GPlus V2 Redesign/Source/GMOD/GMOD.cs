using GPlus.Source.Enums;
using GPlus.Source.Interprocess;
using GPlus.Source.Structs;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace GPlus.Source.GMOD
{
    internal class GMOD
    {
        public Process Process { get; set; }
        private CommunicationManager mgr;
        public static GMOD Instance;

        // Pending requests waiting for responses
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<string>> pending =
            new ConcurrentDictionary<Guid, TaskCompletionSource<string>>();

        public GMOD() => Instance = this;

        private void HandleOutput(object? sender, (int pid, string msg) e)
        {
            try
            {
                // Could be a request or a response, here we assume response
                var resp = System.Text.Json.JsonSerializer.Deserialize<GMODResponse>(e.msg);
                if (resp != null && pending.TryRemove(resp.RequestId, out var tcs))
                {
                    tcs.SetResult(resp.Result);
                    return;
                }

                // If it's not a response, handle as unsolicited message from DLL
                Debug.WriteLine($"[GMOD] Unsolicited message: {e.msg}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GMOD] Failed to parse message: {ex.Message}");
            }
        }

        private async Task<string?> SendRequestAsync(GMODRequest request)
        {
            if (request.ExpectResponse)
            {
                var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
                pending[request.RequestId] = tcs;
                await mgr.SendAsync(Process.Id, request.ToJson());

                // Timeout after 10s if no response
                var completed = await Task.WhenAny(tcs.Task, Task.Delay(10000));
                if (completed == tcs.Task)
                    return tcs.Task.Result;

                pending.TryRemove(request.RequestId, out _);
                throw new TimeoutException("GMOD request timed out");
            }
            else
            {
                await mgr.SendAsync(Process.Id, request.ToJson());
                return null;
            }
        }

        public async Task<bool> Ping()
        {
            var resp = await SendRequestAsync(new GMODRequest
            {
                Type = GMODRequestTypes.PING,
                ExpectResponse = true,
            });

            Debug.WriteLine($"Response: {resp}");

            if (resp != null)
                return true;
            return false;
        }

        internal async Task Initialize()
        {
            if (Process == null)
                throw new Exception("Tried to initialize gmod class with no process");

            mgr = new CommunicationManager();
            _ = mgr.EnsureListeningAsync(Process.Id);

            mgr.MessageReceived += HandleOutput;
        }

        public static async Task OnShutdown()
        {
            Instance.mgr.Dispose();
        }

        public void Shutdown() // quit gmod
        {
            // Send quit request to DLL if needed
            _ = SendRequestAsync(new GMODRequest
            {
                Type = GMODRequestTypes.QUIT,
                ExpectResponse = false
            });
        }
    }
}
