using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NServiceBus;
using NServiceBus.MessageInterfaces;
using NServiceBus.Serialization;
using NewtonSerializer = Newtonsoft.Json.JsonSerializer;

internal class JsonMessageSerializer : IMessageSerializer
{
    IMessageMapper messageMapper;
    MessageContractResolver messageContractResolver;
    Func<Stream, JsonReader> readerCreator;
    JsonSerializerSettings settings;
    Func<Stream, JsonWriter> writerCreator;

    /// <summary>
    /// Initializes a new instance of <see cref="JsonMessageSerializer"/>.
    /// </summary>
    public JsonMessageSerializer(IMessageMapper messageMapper)
    {
        Guard.AgainstNull(messageMapper, "messageMapper");
        this.messageMapper = messageMapper;
        messageContractResolver = new MessageContractResolver(messageMapper);
        Settings = new JsonSerializerSettings
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
        WriterCreator = stream =>
        {
            var streamWriter = new StreamWriter(stream, Encoding.UTF8);
            return new JsonTextWriter(streamWriter)
            {
                Formatting = Formatting.None
            };
        };
        ReaderCreator = stream =>
        {
            var streamReader = new StreamReader(stream, Encoding.UTF8);
            return new JsonTextReader(streamReader);
        };
    }

    public void Serialize(object message, Stream stream)
    {
        Guard.AgainstNull(stream, "stream");
        Guard.AgainstNull(message, "message");
        var jsonSerializer = NewtonSerializer.Create(Settings);
        jsonSerializer.Binder = new MessageSerializationBinder(messageMapper);
        var jsonWriter = WriterCreator(stream);
        jsonSerializer.Serialize(jsonWriter, message);
        jsonWriter.Flush();
    }

    public object[] Deserialize(Stream stream, IList<Type> messageTypes)
    {
        Guard.AgainstNull(stream, "stream");
        Guard.AgainstNull(messageTypes, "messageTypes");
        var jsonSerializer = NewtonSerializer.Create(Settings);
        jsonSerializer.ContractResolver = messageContractResolver;
        jsonSerializer.Binder = new MessageSerializationBinder(messageMapper, messageTypes);

        if (IsArrayStream(stream))
        {
            var arrayReader = ReaderCreator(stream);
            return jsonSerializer.Deserialize<object[]>(arrayReader);
        }

        if (messageTypes.Any())
        {
            return DeserializeMultipleMesageTypes(stream, messageTypes, jsonSerializer).ToArray();
        }

        var simpleReader = ReaderCreator(stream);
        return new[]
        {
            jsonSerializer.Deserialize<object>(simpleReader)
        };
    }

    IEnumerable<object> DeserializeMultipleMesageTypes(Stream stream, IList<Type> messageTypes, NewtonSerializer jsonSerializer)
    {
        foreach (var messageType in FindRootTypes(messageTypes))
        {
            stream.Seek(0, SeekOrigin.Begin);
            var reader = ReaderCreator(stream);
            yield return jsonSerializer.Deserialize(reader, messageType);
        }
    }

    bool IsArrayStream(Stream stream)
    {
        var reader = ReaderCreator(stream);
        reader.Read();
        stream.Seek(0, SeekOrigin.Begin);
        return reader.TokenType == JsonToken.StartArray;
    }

    static IEnumerable<Type> FindRootTypes(IEnumerable<Type> messageTypesToDeserialize)
    {
        Type currentRoot = null;
        foreach (var type in messageTypesToDeserialize)
        {
            if (currentRoot == null)
            {
                currentRoot = type;
                yield return currentRoot;
                continue;
            }
            if (!type.IsAssignableFrom(currentRoot))
            {
                currentRoot = type;
                yield return currentRoot;
            }
        }
    }

    public string ContentType
    {
        get { return ContentTypes.Json; }
    }

    /// <summary>
    /// Configures the <see cref="JsonSerializerSettings"/> to use.
    /// </summary>
    public JsonSerializerSettings Settings
    {
        get { return settings; }
        set
        {
            Guard.AgainstNull(value, "value");
            settings = value;
        }
    }

    /// <summary>
    /// Configures the <see cref="JsonReader"/> creator of JSON stream.
    /// </summary>
    public Func<Stream, JsonReader> ReaderCreator
    {
        get { return readerCreator; }
        set
        {
            Guard.AgainstNull(value, "value");
            readerCreator = value;
        }
    }

    /// <summary>
    /// Configures the <see cref="JsonWriter"/> creator of JSON stream.
    /// </summary>
    public Func<Stream, JsonWriter> WriterCreator
    {
        get { return writerCreator; }
        set
        {
            Guard.AgainstNull(value, "value");
            writerCreator = value;
        }
    }
}