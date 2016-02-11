using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static void Main()
    {
        var endpointConfiguration = new EndpointConfiguration();
        endpointConfiguration.EndpointName("NewtonsoftSerializerSample");
        endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.UsePersistence<InMemoryPersistence>();
        Run(endpointConfiguration).GetAwaiter().GetResult();
    }

    static async Task Run(EndpointConfiguration endpointConfiguration)
    {
        var endpoint = await Endpoint.Start(endpointConfiguration);
        await endpoint.SendLocal(new MyMessage
        {
            DateSend = DateTime.Now,
        });
        Console.WriteLine("\r\nPress any key to stop program\r\n");
        Console.Read();
    }
}