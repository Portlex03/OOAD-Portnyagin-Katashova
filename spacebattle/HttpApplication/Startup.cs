using CoreWCF;
using CoreWCF.Configuration;
using Swashbuckle.AspNetCore.Swagger;
using WebHttp;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal sealed class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddServiceModelWebServices(o =>
        {
            o.Title = "SpaceBattle API";
            o.Version = "1.0";
            o.Description = "Отправляет команды игровым объектам";
        });

        services.AddSingleton(new SwaggerOptions());
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseMiddleware<SwaggerMiddleware>();
        app.UseSwaggerUI();

        app.UseServiceModel(builder =>
        {
            builder.AddService<WebApi>();
            builder.AddServiceWebEndpoint<WebApi, IWebApi>(new WebHttpBinding
            {
                MaxReceivedMessageSize = 5242880,
                MaxBufferSize = 65536,
            }, "api", behavior =>
            {
                behavior.HelpEnabled = true;
                behavior.AutomaticFormatSelectionEnabled = true;
            });
        });
    }
}
