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
        var bus = await Endpoint.Start(busConfig);
        var session = bus.CreateBusSession();
        await session.SendLocal(new MyMessage
        {
            DateSend = DateTime.Now,
        });
        Console.WriteLine("\r\nPress any key to stop program\r\n");
        Console.Read();
    }
}