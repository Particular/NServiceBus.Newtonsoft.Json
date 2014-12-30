![Icon](https://raw.githubusercontent.com/SimonCropp/NServiceBus.Newtonsoft.Json/master/Icon/package_icon.png)

NServiceBus.Newtonsoft.Json
===========================

Add support for [NServiceBus](http://particular.net/NServiceBus) message serialization via[Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)

## Nuget

### http://nuget.org/packages/NServiceBus.Newtonsoft.Json/

    PM> Install-Package NServiceBus.Newtonsoft.Json

## Usage

```
var busConfig = new BusConfiguration();
busConfig.UseSerialization<NewtonsoftSerializer>();
```

### Json.net attributes

Json.net attributes are supported.

For example

```
[JsonObject(MemberSerialization.OptIn)]
public class CreatePersonMessage : IMessage
{
    // "John Smith"
    [JsonProperty]
    public string Name { get; set; }

    // "2000-12-15T22:11:03"
    [JsonProperty]
    public DateTime BirthDate { get; set; }

    // new Date(976918263055)
    [JsonProperty]
    [JsonConverter(typeof(JavaScriptDateTimeConverter))]
    public DateTime LastModified { get; set; }

    // not serialized because mode is opt-in
    public string Department { get; set; }
}
```

### Custom Settings

Customizes the instance of `JsonSerializerSettings` used for serialization.

```
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
busConfig.UseSerialization<NewtonsoftSerializer>()
    .Settings(settings);
```

### Custom Reader

Customize the creation of the `JsonReader`.

```
var busConfig = new BusConfiguration();
busConfig.UseSerialization<NewtonsoftSerializer>()
    .ReaderCreator(stream =>
    {
        var streamReader = new StreamReader(stream, Encoding.UTF8);
        return new JsonTextReader(streamReader);
    });
```

### Custom Writer

Customize the creation of the `JsonWriter`.

```
var busConfig = new BusConfiguration();
busConfig.UseSerialization<NewtonsoftSerializer>()
    .WriterCreator(stream =>
    {
        var streamWriter = new StreamWriter(stream, Encoding.UTF8);
        return new JsonTextWriter(streamWriter)
               {
                   Formatting = Formatting.None
               };
    });
```

## Icon

<a href="http://thenounproject.com/term/chess/15459/" target="_blank">Chess</a> designed by <a href="http://thenounproject.com/diegonaive/" target="_blank">Diego Naive</a> from The Noun Project
