using System.Buffers.Binary;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using static GPlus.Source.Interprocess.Memory;

namespace GPlus.Source.Interprocess
{
    /// <summary>
    /// Represents a single named-pipe server that accepts exactly one connection from the injected DLL.
    /// Messages are length-prefixed (int32 little-endian) UTF-8 payloads.
    /// </summary>
    public class Communication : IDisposable
    {
        public int TargetPid { get; }
        public string PipeName { get; }

        private readonly NamedPipeServerStream _server;
        private readonly CancellationTokenSource _cts = new();
        private readonly SemaphoreSlim _writeLock = new(1, 1);

        private Stream? _stream;
        private Task? _readerTask;

        public bool IsConnected => _server.IsConnected;

        public event EventHandler<string>? MessageReceived;
        public event EventHandler? Connected;
        public event EventHandler? Disconnected;
        public event EventHandler<Exception>? Faulted;

        public Communication(int targetPid, int maxBufferSize = 64 * 1024)
        {
            TargetPid = targetPid;
            PipeName = $"gplus_comm_pipe_{targetPid}";

            // Single instance, message mode, async
            _server = new NamedPipeServerStream(
                PipeName,
                PipeDirection.InOut,
                maxNumberOfServerInstances: 1,
                transmissionMode: PipeTransmissionMode.Byte,
                options: PipeOptions.Asynchronous);
        }

        /// <summary>
        /// Waits for the client (DLL) to connect and starts the receive loop.
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            var linked = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellationToken);
            try
            {
                await _server.WaitForConnectionAsync(linked.Token).ConfigureAwait(false);
                _stream = _server; // use raw stream
                Connected?.Invoke(this, EventArgs.Empty);

                _readerTask = Task.Run(() => ReadLoopAsync(linked.Token), linked.Token);
            }
            catch (OperationCanceledException) { /* cancelled */ }
            catch (Exception ex)
            {
                Faulted?.Invoke(this, ex);
            }
        }

        private async Task ReadLoopAsync(CancellationToken ct)
        {
            try
            {
                var lengthBuf = new byte[4];
                while (!ct.IsCancellationRequested && _server.IsConnected)
                {
                    // Read exactly 4 bytes for length
                    int read = await ReadExactlyAsync(_stream!, lengthBuf, 0, 4, ct).ConfigureAwait(false);
                    if (read == 0) break; // disconnected

                    int payloadLen = BinaryPrimitives.ReadInt32LittleEndian(lengthBuf);
                    if (payloadLen <= 0)
                        continue;

                    var buf = new byte[payloadLen];
                    await ReadExactlyAsync(_stream!, buf, 0, payloadLen, ct).ConfigureAwait(false);

                    var msg = Encoding.UTF8.GetString(buf);
                    MessageReceived?.Invoke(this, msg);
                }
            }
            catch (OperationCanceledException) { /* stopped */ }
            catch (Exception ex)
            {
                Faulted?.Invoke(this, ex);
            }
            finally
            {
                TryDisconnect();
            }
        }

        private static async Task<int> ReadExactlyAsync(Stream s, byte[] buffer, int offset, int count, CancellationToken ct)
        {
            int total = 0;
            while (total < count)
            {
                int n = await s.ReadAsync(buffer, offset + total, count - total, ct).ConfigureAwait(false);
                if (n == 0) return 0; // remote closed
                total += n;
            }
            return total;
        }

        /// <summary>
        /// Sends a UTF-8 message using length-prefixed framing.
        /// </summary>
        public async Task SendAsync(string message, CancellationToken ct = default)
        {
            if (_stream == null || !_server.IsConnected)
                throw new InvalidOperationException("Not connected.");

            var payload = Encoding.UTF8.GetBytes(message);
            var header = new byte[4];
            BinaryPrimitives.WriteInt32LittleEndian(header, payload.Length);

            await _writeLock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                await _stream.WriteAsync(header, 0, 4, ct).ConfigureAwait(false);
                await _stream.WriteAsync(payload, 0, payload.Length, ct).ConfigureAwait(false);
                await _stream.FlushAsync(ct).ConfigureAwait(false);
            }
            finally
            {
                _writeLock.Release();
            }
        }

        /// <summary>
        /// Stops and disposes the connection.
        /// </summary>
        public void Stop()
        {
            _cts.Cancel();
            TryDisconnect();
        }

        private void TryDisconnect()
        {
            try
            {
                if (_server.IsConnected)
                    _server.Disconnect();
            }
            catch { }
            finally
            {
                _stream?.Dispose();
                _readerTask = null;
                Disconnected?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            Stop();
            _server.Dispose();
            _cts.Dispose();
            _writeLock.Dispose();
        }
    }
}
