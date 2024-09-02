using System.Net.WebSockets;
using System.Text;

public class HaikuLogger
{
    private readonly ClientWebSocket _clientWebSocket;
    private readonly Uri _librarianUri;

    public HaikuLogger(string librarianUrl)
    {
        _clientWebSocket = new ClientWebSocket();
        _librarianUri = new Uri(librarianUrl);
    }

    public async Task ConnectAsync()
    {
        await _clientWebSocket.ConnectAsync(_librarianUri, CancellationToken.None);
    }

    public async Task LogHaikuAsync(string haiku)
    {
        try
        {
            var haikuMessage = $"Haiku: {haiku}";
            var buffer = Encoding.UTF8.GetBytes(haikuMessage);
            var segment = new ArraySegment<byte>(buffer);

            await _clientWebSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception while sending haiku: {ex.Message}");
        }
    }
}
