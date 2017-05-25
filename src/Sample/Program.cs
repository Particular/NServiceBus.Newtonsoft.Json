using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static void Main()
    {
        var endpointConfiguration = new EndpointConfiguration("NewtonsoftSerializerSample");
        endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UseTransport<LearningTransport>();
        endpointConfiguration.UsePersistence<InMemoryPersistence>();
        Run(endpointConfiguration).GetAwaiter().GetResult();
    }

    static async Task Run(EndpointConfiguration endpointConfiguration)
    {
        var endpoint = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        var myMessage = new MyMessage
        {
            DateSend = DateTime.Now,
        };
        await endpoint.SendLocal(myMessage)
            .ConfigureAwait(false);
        Console.WriteLine("\r\nPress any key to stop program\r\n");
        Console.Read();
    }
}