using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NServiceBus;
using NServiceBus.Newtonsoft.Json;

// ReSharper disable UnusedMember.Local

class Samples
{
    static void Settings()
    {
        var settings = new JsonSerializerSettings
                       {
                           TypeNameHandling = TypeNameHandling.Auto,
                           Converters =
                           {
                               new IsoDateTimeConverter
                               {
                                   DateTimeStyles = DateTimeStyles.RoundtripKind
                               }
                           }
                       };
        var busConfig = new BusConfiguration();
        busConfig.UseSerialization<NewtonsoftSerialization>()
            .Settings(settings);
    }

    static void Writer()
    {
        var busConfig = new BusConfiguration();
        busConfig.UseSerialization<NewtonsoftSerialization>()
            .WriterCreator(stream =>
            {
                var streamWriter = new StreamWriter(stream, Encoding.UTF8);
                return new JsonTextWriter(streamWriter)
                       {
                           Formatting = Formatting.None
                       };
            });
    }

    static void Reader()
    {
        var busConfig = new BusConfiguration();
        busConfig.UseSerialization<NewtonsoftSerialization>()
            .ReaderCreator(stream =>
            {
                var streamReader = new StreamReader(stream, Encoding.UTF8);
                return new JsonTextReader(streamReader);
            });
    }
}