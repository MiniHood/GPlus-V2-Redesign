using GPlus.Game.Clients;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace GPlus.Source.Interprocess
{
    public static class CommunicationTCP
    {
        private static TcpListener listener;
        public static ConcurrentDictionary<int, TcpClient> ConnectedClients = new(); // GMOD PID -> TcpClient

        public static async Task StartAsync(int port = 8080)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Debug.WriteLine("TCP Server started.");

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        }

        private static async Task HandleClientAsync(TcpClient client)
        {
            var stream = client.GetStream();
            var buffer = new byte[4096];
            int? gmodPid = null;

            try
            {
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string json = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    Debug.WriteLine($"Received JSON: {json}");

                    try
                    {
                        using var doc = JsonDocument.Parse(json);
                        var root = doc.RootElement;

                        if (root.TryGetProperty("PID", out var pidProp))
                        {
                            gmodPid = pidProp.GetInt32();
                            ConnectedClients.TryAdd(gmodPid.Value, client);
                        }

                        if (root.TryGetProperty("LuaReady", out var luaProp) && gmodPid.HasValue)
                        {
                            var clientInstance = ClientManager.GetClientByGMODPid(gmodPid.Value);
                            if (clientInstance != null)
                                clientInstance.GMOD.LuaReady = luaProp.GetBoolean();
                        }

                        if (root.TryGetProperty("Responses", out var responses) && responses.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var resp in responses.EnumerateArray())
                            {
                                string command = resp.GetProperty("Command").GetString() ?? "";
                                string result = resp.GetProperty("Result").GetString() ?? "";
                                bool success = resp.GetProperty("Success").GetBoolean();
                                Debug.WriteLine($"Client {gmodPid} executed: {command}, success={success}, result={result}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to parse JSON: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Client error: {ex.Message}");
            }
            finally
            {
                if (gmodPid.HasValue)
                {
                    ConnectedClients.TryRemove(gmodPid.Value, out _);
                    var clientInstance = ClientManager.GetClientByGMODPid(gmodPid.Value);
                    if (clientInstance != null)
                        clientInstance.GMOD.LuaReady = false;
                }
                client.Close();
            }
        }


        public static async Task SendCommandToClient(int pid, string type, string data)
        {
            if (ConnectedClients.TryGetValue(pid, out var client))
            {
                var msg = new
                {
                    Commands = new[]
                    {
                new { Type = type, Data = data }
            }
                };

                string json = JsonSerializer.Serialize(msg) + "\n";
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                await client.GetStream().WriteAsync(bytes, 0, bytes.Length);
            }
        }

    }
}
