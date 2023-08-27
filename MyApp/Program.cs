using ServiceStack;
using ServiceStack.Messaging;
using ServiceStack.Azure.Messaging;
using MyApp.ServiceModel;

namespace MyApp;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args)
            .Build()
            .UseServiceStack(new GenericAppHost(typeof(MyService).Assembly)
            {
                ConfigureAppHost = host =>
                {
                    var mqServer = host.Resolve<IMessageService>();
                    mqServer.RegisterHandler<Hello>(host.ExecuteMessage);
                }
            })
            .Run();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IMessageService>(new ServiceBusMqServer(
                    hostContext.Configuration.GetConnectionString("ServiceBus")) {
                    DisablePublishingToOutq = true,
                });
                services.AddHostedService<MqWorker>();
            });
}
