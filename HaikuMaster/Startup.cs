using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.WebSockets;

public class Startup
{
    private static WordsSender _wordsSender;
    private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public void ConfigureServices(IServiceCollection services)
    {
        // Add necessary services
        services.AddControllers(); // Register controllers if needed
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers(); // Map controllers if needed
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("WebSocket server is running.");
            });
        });

        // Initialize WordsSender
        _wordsSender = new WordsSender("C:\\Users\\Admin\\Documents\\New folder\\OSProject\\HaikuMaster\\words.txt", 1000);

        app.UseWebSockets();
        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws" && context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                _wordsSender.AddClient(webSocket);

                await HandleWebSocket(webSocket);
            }
            else
            {
                await next();
            }
        });

        // Start sending words
        Task.Run(() => _wordsSender.StartSendingWords(_cancellationTokenSource.Token));
    }

    private async Task HandleWebSocket(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];

        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                _wordsSender.RemoveClient(webSocket);
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
            // You can handle other message types here if needed
        }
    }
}
