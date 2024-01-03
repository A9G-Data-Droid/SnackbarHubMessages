// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClientHello;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            IHost helloHost = Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging
                        .AddDebug()
                        .AddConsole();
                })
                .UseWindowsService(options => { options.ServiceName = "Hello"; })
                .ConfigureServices(ConfigureServices)
                .Build();

            await helloHost.RunAsync();
        }
        catch (Exception ex)
        {
            return ex.HResult;
        }

        return 0;
    }


    /// <summary>
    /// Configure all the dependencies, load the settings. 
    /// </summary>
    /// <param name="services"></param>
    public static void ConfigureServices(IServiceCollection services)
    {
        _ = services
                .AddHostedService<TheHubClient>()
            ;
    }
}