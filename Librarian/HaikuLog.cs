namespace HaikuLibrarian
{
    public class HaikuLog
    {
        private readonly string _logFilePath;

        public HaikuLog()
        {
            _logFilePath = "LibrarianHaikuLog.txt";
        }

        public void LogHaiku(string haiku)
        {
            var logEntry = $"[{DateTime.Now}] Received Haiku:\n{haiku}\n-----------------\n";
            File.AppendAllText(_logFilePath, logEntry);
            Console.WriteLine($"Haiku logged:\n{logEntry}");
        }
    }
}
