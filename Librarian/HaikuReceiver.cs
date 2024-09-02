namespace HaikuLibrarian
{
    public class HaikuReceiver
    {
        private readonly HaikuLog _haikuLog;

        public HaikuReceiver()
        {
            _haikuLog = new HaikuLog();
        }

        public void ReceiveHaiku(string haiku)
        {
            Console.WriteLine("Haiku received:");
            Console.WriteLine(haiku);
            Console.WriteLine("Logging the haiku...");
            _haikuLog.LogHaiku(haiku);
        }
    }
}
