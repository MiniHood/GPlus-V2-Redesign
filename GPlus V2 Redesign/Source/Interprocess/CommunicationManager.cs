using GPlus.Source.Interprocess;

using System.Collections.Concurrent;

namespace GPlus.Source.Interprocess
{
    /// <summary>
    /// Manages multiple Communication servers keyed by PID.
    /// </summary>
    public class CommunicationManager : IDisposable
    {
        private readonly ConcurrentDictionary<int, Communication> _comms = new();
        private readonly CancellationTokenSource _cts = new();

        public event EventHandler<(int pid, string msg)>? MessageReceived;
        public event EventHandler<int>? ClientConnected;
        public event EventHandler<int>? ClientDisconnected;
        public event EventHandler<(int pid, Exception ex)>? ClientFaulted;

        /// <summary>
        /// Ensure a server is listening for the given PID. Safe to call multiple times.
        /// </summary>
        public async Task<Communication> EnsureListeningAsync(int pid, CancellationToken ct = default)
        {
            var comm = _comms.GetOrAdd(pid, p =>
            {
                var c = new Communication(p);
                c.MessageReceived += (_, msg) => MessageReceived?.Invoke(this, (p, msg));
                c.Connected += (_, __) => ClientConnected?.Invoke(this, p);
                c.Disconnected += (_, __) => ClientDisconnected?.Invoke(this, p);
                c.Faulted += (_, ex) => ClientFaulted?.Invoke(this, (p, ex));
                return c;
            });

            // Start if not started
            // (StartAsync is idempotent-ish: WaitForConnectionAsync will return if already waiting/connected)
            await comm.StartAsync(ct).ConfigureAwait(false);
            return comm;
        }

        public bool IsListening(int pid) => _comms.ContainsKey(pid) && _comms[pid].IsConnected;

        public async Task SendAsync(int pid, string message, CancellationToken ct = default)
        {
            if (_comms.TryGetValue(pid, out var comm))
            {
                await comm.SendAsync(message, ct).ConfigureAwait(false);
            }
            else
            {
                throw new InvalidOperationException("No server for pid. Call EnsureListeningAsync first.");
            }
        }

        public async Task BroadcastAsync(string message, CancellationToken ct = default)
        {
            var tasks = new List<Task>();
            foreach (var kv in _comms)
            {
                tasks.Add(kv.Value.SendAsync(message, ct));
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public void StopListening(int pid)
        {
            if (_comms.TryRemove(pid, out var comm))
            {
                comm.Dispose();
            }
        }

        public void StopAll()
        {
            foreach (var kv in _comms.Keys)
                StopListening(kv);
        }

        public void Dispose()
        {
            StopAll();
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}