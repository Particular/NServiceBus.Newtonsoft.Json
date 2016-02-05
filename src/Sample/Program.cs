using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Newtonsoft.Json;

class Program
{
    static void Main()
    {
        var busConfig = new BusConfiguration();
        busConfig.EndpointName("NewtonsoftSerializerSample");
        busConfig.UseSerialization<NewtonsoftSerialization>();
        busConfig.EnableInstallers();
        busConfig.UsePersistence<InMemoryPersistence>();
        Run(busConfig).GetAwaiter().GetResult();
    }

    private static async Task Run(BusConfiguration busConfig)
    {
        var endpoint = await Endpoint.Start(busConfig);
        await endpoint.SendLocal(new MyMessage
        {
            DateSend = DateTime.Now,
        });
        Console.WriteLine("\r\nPress any key to stop program\r\n");
        Console.Read();
    }
}