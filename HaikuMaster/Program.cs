namespace HaikuMaster;
public class Program
{
    static void Main(string[] args)
    {
        HaikuMasterManager masterManager = new HaikuMasterManager();

        Console.WriteLine("Haiku Master Daemon Started.");
        Console.WriteLine("Available commands:");
        Console.WriteLine("add master <port> <cooldown>");
        Console.WriteLine("remove master <port>");
        Console.WriteLine("list masters");
        Console.WriteLine("exit");

        while (true)
        {
            var input = Console.ReadLine()?.Split(' ');
            if (input == null || input.Length == 0)
                continue;

            switch (input[0].ToLower())
            {
                case "add":
                    if (input.Length == 3 && input[1] == "master" &&
                        int.TryParse(input[2], out var port))
                    {
                        masterManager.AddMaster(port);
                    }
                    else
                    {
                        Console.WriteLine("Usage: add master <port> <dictionaryPath> <cooldown>");
                    }
                    break;

                case "remove":
                    if (input.Length == 3 && input[1] == "master" && int.TryParse(input[2], out var removePort))
                    {
                        masterManager.RemoveMaster(removePort);
                    }
                    else
                    {
                        Console.WriteLine("Usage: remove master <port>");
                    }
                    break;

                case "list":
                    masterManager.ListMasters();
                    break;

                case "exit":
                    masterManager.StopAllMasters();
                    return;

                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }
    }
}
