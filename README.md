![Icon](https://raw.github.com/SimonCropp/NServiceBus.Newtonsoft.Json/master/Icons/package_icon.png)

NServiceBus.Newtonsoft.Json
===========================

Add support for sending [NServiceBus](http://particular.net/NServiceBus) logging message through [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)

## Nuget

### http://nuget.org/packages/NServiceBus.Newtonsoft.Json/

    PM> Install-Package NServiceBus.Newtonsoft.Json

## Usage

```
var busConfig = new BusConfiguration();
busConfig.UseSerialization<NewtonsoftSerializer>();
```

### Json.net attributes

Json.net attributes are supported

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
