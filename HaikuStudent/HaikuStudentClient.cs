using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HaikuStudent
{
    public class HaikuStudentClient
    {
        private readonly string _hostname;
        private readonly List<int> _ports;
        private readonly List<ClientWebSocket> _sockets = new List<ClientWebSocket>();
        private bool _running;

        private readonly HaikuLogger _haikuLogger;
        enum HaikuLine { First, Second, Third }
        HaikuLine currentLine = HaikuLine.First;

        public HaikuStudentClient(string hostname, List<int> ports, string librarianPath)
        {
            _hostname = hostname;
            _ports = ports;
            _haikuLogger = new HaikuLogger(librarianPath);
        }

        public void Start()
        {
            _running = true;

            // Start listening on all ports
            foreach (var port in _ports)
            {
                var socket = new ClientWebSocket();
                _sockets.Add(socket);
                Task.Run(() => ConnectToMaster(socket, port));
            }
        }

        public async Task ConnectToMaster(ClientWebSocket socket, int port)
        {
            var uri = new Uri($"ws://{_hostname}:{port}/ws");

            using (var webSocket = new ClientWebSocket())
            {
                try
                {
                    await webSocket.ConnectAsync(uri, CancellationToken.None);
                    Console.WriteLine($"Connected to {uri}");

                    await _haikuLogger.ConnectAsync();

                    var buffer = new byte[1024 * 4];
                    var stringBuilder = new StringBuilder();
                    int wordCount = 0;
                    string firstLine = string.Empty, secondLine = string.Empty;


                    while (webSocket.State == WebSocketState.Open)
                    {
                        //var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        //if (result.MessageType == WebSocketMessageType.Close)
                        //{
                        //    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        //    Console.WriteLine($"Connection closed by server on port {port}.");
                        //}
                        //else if (result.MessageType == WebSocketMessageType.Text)
                        //{
                        //    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        //    Console.WriteLine($"Received word: {message}");
                        //}
                        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        }
                        else
                        {
                            // Collect words and form Haikus
                            stringBuilder.Append($"{message} ");
                            wordCount++;

                            if (wordCount == 5) // End of a 5-word line
                            {
                                if (currentLine == HaikuLine.First)
                                {
                                    firstLine = stringBuilder.ToString();
                                    Console.WriteLine($"First line completed: {firstLine}");
                                    currentLine = HaikuLine.Second;
                                    wordCount = 0;
                                    stringBuilder.Clear();
                                }
                                else if (currentLine == HaikuLine.Second)
                                {
                                    secondLine = stringBuilder.ToString();
                                    Console.WriteLine($"Second line completed: {secondLine}");
                                    currentLine = HaikuLine.Third;
                                    wordCount = 0;
                                    stringBuilder.Clear();
                                }
                            }
                            else if (wordCount == 7) // End of a 7-word line
                            {
                                if (currentLine == HaikuLine.Third)
                                {
                                    var thirdLine = stringBuilder.ToString();
                                    Console.WriteLine($"Third line completed: {thirdLine}");
                                    stringBuilder.Clear();
                                    wordCount = 0;
                                    // Form the complete Haiku
                                    string haiku = $"{firstLine}\n{secondLine}\n{thirdLine}";
                                    // Log the complete Haiku
                                    await _haikuLogger.LogHaikuAsync(haiku);
                                    // Reset for next Haiku
                                    firstLine = string.Empty;
                                    secondLine = string.Empty;
                                    currentLine = HaikuLine.First;
                                }
                                else
                                {
                                    // If we are not in the third line but reached 7 words, clear buffer
                                    stringBuilder.Clear();
                                    wordCount = 0;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error connecting to WebSocket: {ex.Message}");
                }
            }
        }

        public void Stop()
        {
            _running = false;

            // Close all sockets
            foreach (var socket in _sockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).Wait();
                }
            }

            Console.WriteLine("Haiku Student stopped.");
        }
    }
}
