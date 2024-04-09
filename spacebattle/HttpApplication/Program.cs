using Microsoft.AspNetCore;

IWebHostBuilder builder = WebHost.CreateDefaultBuilder(args)
    .UseKestrel(options =>
    { 
        options.ListenAnyIP(8080); 
    })
    .UseStartup<Startup>();

IWebHost app = builder.Build();
app.Run();
