namespace HaikuMaster;
public class HaikuMasterManager
{
    private readonly Dictionary<int, HaikuMasterInstance> _masters = new Dictionary<int, HaikuMasterInstance>();

    public void AddMaster(int port)
    {
        if (_masters.ContainsKey(port))
        {
            Console.WriteLine($"Master on port {port} already exists.");
            return;
        }

        var haikuMasterInstance = new HaikuMasterInstance(port);
        _masters.Add(port, haikuMasterInstance);
        Task.Run(() => haikuMasterInstance.Start());

        Console.WriteLine($"Master added on port {port}");
    }

    public void RemoveMaster(int port)
    {
        if (_masters.TryGetValue(port, out var instance))
        {
            Console.WriteLine($"Shutting down Master on port {port}...");
            Task.Run(() => instance.StopAsync()).Wait();
            _masters.Remove(port);
        }
        else
        {
            Console.WriteLine($"No Master found on port {port}.");
        }
    }

    public void ListMasters()
    {
        Console.WriteLine("Active Masters:");
        foreach (var port in _masters.Keys)
        {
            Console.WriteLine($"- Master running on port {port}");
        }
    }

    public void StopAllMasters()
    {
        foreach (var instance in _masters.Values)
        {
            Task.Run(() => instance.StopAsync()).Wait();
        }
        _masters.Clear();
        Console.WriteLine("All Masters stopped.");
    }
}
