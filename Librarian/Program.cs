namespace HaikuLibrarian
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Librarian...");

            int port = 8091;
            if (args.Length > 0 && int.TryParse(args[0], out int providedPort))
            {
                port = providedPort;
            }

            LibrarianServer librarianServer = new LibrarianServer(port);
            librarianServer.Start();

            Console.WriteLine($"Librarian running on port {port}. Press any key to stop...");
            Console.ReadKey();

            librarianServer.Stop();
        }
    }
}
