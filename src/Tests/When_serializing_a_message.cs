using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class When_serializing_a_message
{
    [Test]
    public void Should_emit_UTF8_BOM_by_default()
    {
        var messageMapper = new MessageMapper();
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null);
        var message = new SimpleMessage();
        using (var stream = new MemoryStream())
        {
            serializer.Serialize(message, stream);

            stream.Position = 0;

            var result = stream.ToArray();
            var utf8bom = new UTF8Encoding(true).GetPreamble();

            for (var i = 0; i < utf8bom.Length; i++)
            {
                Assert.AreEqual(utf8bom[i], result[i]);
            }
        }
    }
    
    [Test]
    public void Should_not_emit_UTF8_BOM_if_configured_not_to()
    {
        var messageMapper = new MessageMapper();

        Func<Stream, JsonWriter> writerCreator = stream =>
        {
            var streamWriter = new StreamWriter(stream, new UTF8Encoding(false, false));
            return new JsonTextWriter(streamWriter)
            {
                Formatting = Formatting.None
            };
        };

        var serializer = new JsonMessageSerializer(messageMapper, null, writerCreator, null, null);
        
        var message = new SimpleMessage();
        using (var stream = new MemoryStream())
        {
            serializer.Serialize(message, stream);

            stream.Position = 0;

            var result = stream.ToArray();
            var utf8bom = new UTF8Encoding(true).GetPreamble();

            for (var i = 0; i < utf8bom.Length; i++)
            {
                Assert.AreNotEqual(utf8bom[i], result[i]);
            }
        }
    }

    public class SimpleMessage
    {
        public string SomeProperty { get; set; }
    }

}