using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace HaikuLibrarian
{
    public class LibrarianServer
    {
        private readonly int _port;
        private HttpListener _listener;
        private bool _running;

        private readonly HaikuReceiver _haikuReceiver;

        public LibrarianServer(int port)
        {
            _port = port;
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{_port}/");
            _haikuReceiver = new HaikuReceiver();
        }

        public void Start()
        {
            _running = true;
            _listener.Start();
            Console.WriteLine($"Librarian started, listening on port {_port}.");

            Task.Run(() => AcceptConnections());
        }

        public void Stop()
        {
            _running = false;
            _listener.Stop();
            Console.WriteLine("Librarian stopped.");
        }

        private async Task AcceptConnections()
        {
            while (_running)
            {
                try
                {
                    HttpListenerContext context = await _listener.GetContextAsync();

                    if (context.Request.IsWebSocketRequest)
                    {
                        HttpListenerWebSocketContext wsContext = await context.AcceptWebSocketAsync(null);
                        Task.Run(() => HandleWebSocket(wsContext.WebSocket));
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accepting connection: {ex.Message}");
                }
            }
        }

        private async Task HandleWebSocket(WebSocket socket)
        {
            Console.WriteLine("Student connected.");
            var buffer = new byte[1024 * 4];

            while (_running && socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string haiku = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    _haikuReceiver.ReceiveHaiku(haiku);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    Console.WriteLine("Student disconnected.");
                }
            }
        }
    }
}
