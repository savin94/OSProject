using System.Net.WebSockets;
using System.Text;

public class WordsSender
{
    private readonly List<WebSocket> _clients = new List<WebSocket>();
    private readonly string[] _words;
    private readonly int _cooldownMilliseconds;
    private int _currentWordIndex = 0;

    public WordsSender(string wordsFilePath, int cooldownMilliseconds)
    {
        _words = File.ReadAllLines(wordsFilePath);
        _cooldownMilliseconds = cooldownMilliseconds;
    }

    public void AddClient(WebSocket client)
    {
        lock (_clients)
        {
            _clients.Add(client);
        }
    }

    public void RemoveClient(WebSocket client)
    {
        lock (_clients)
        {
            _clients.Remove(client);
        }
    }

    public async Task StartSendingWords(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            string word = _words[_currentWordIndex];
            _currentWordIndex = (_currentWordIndex + 1) % _words.Length;

            byte[] buffer = Encoding.UTF8.GetBytes(word);

            foreach (var client in _clients.ToArray()) 
            {
                if (client.State == WebSocketState.Open)
                {
                    await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellationToken);
                }
            }

            await Task.Delay(_cooldownMilliseconds, cancellationToken);
        }
    }
}
