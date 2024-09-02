using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace HaikuMaster;
public class HaikuMasterInstance(int port)
{
    private readonly int _port = port;
    private  IHost _host;

    public void Start()
    {
        _host = Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>()
                                  .UseUrls($"http://localhost:{_port}");
                    })
                    .Build();

        _host.Run();
    }

    public async Task StopAsync()
    {
        if (_host != null)
        {
            await _host.StopAsync();
        }
    }
}
