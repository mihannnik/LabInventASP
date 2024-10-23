using FileParser.Interfaces;
using FileParser.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();
        builder.Services.AddHostedService<FileParserService>();
        

        var app = builder.Build();

        app.Run();
    }
}