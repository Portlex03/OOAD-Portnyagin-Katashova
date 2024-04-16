using Microsoft.AspNetCore;
using System.Diagnostics.CodeAnalysis;

namespace WebHttp;

[ExcludeFromCodeCoverage]
public class Program
{
    static void Main(string[] args)
    {
        IWebHostBuilder builder = WebHost.CreateDefaultBuilder(args)
        .UseKestrel(options =>
        {
            options.ListenAnyIP(8080);
        })
        .UseStartup<Startup>();

        IWebHost app = builder.Build();
        app.Run();
    }
}

