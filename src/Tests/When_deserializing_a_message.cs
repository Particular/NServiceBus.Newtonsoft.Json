using System.IO;
using System.Linq;
using System.Text;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class When_deserializing_a_message
{

    [Test]
    [TestCase(Newtonsoft.Json.TypeNameHandling.Auto)]
    [TestCase(Newtonsoft.Json.TypeNameHandling.None)]
    public void Should_handle_message_with_UTF8_BOM(Newtonsoft.Json.TypeNameHandling typeNameHandling)
    {
        var messageMapper = new MessageMapper();
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null, typeNameHandling);

        var utf8WithBomEncoding = new UTF8Encoding(true);
        var serialized = utf8WithBomEncoding
            .GetPreamble()
            .Concat(utf8WithBomEncoding.GetBytes($"{{\"{nameof(SimpleMessage.SomeProperty)}\":\"John\"}}"))
            .ToArray();

        using (var stream = new MemoryStream(serialized))
        {
            stream.Position = 0;

            var result = (SimpleMessage)serializer.Deserialize(stream, new[] { typeof(SimpleMessage) })[0];

            Assert.AreEqual("John", result.SomeProperty);
        }
    }

    [Test]
    [TestCase(Newtonsoft.Json.TypeNameHandling.Auto)]
    [TestCase(Newtonsoft.Json.TypeNameHandling.None)]
    public void Should_handle_message_without_UTF8_BOM(Newtonsoft.Json.TypeNameHandling typeNameHandling)
    {
        var messageMapper = new MessageMapper();
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null, typeNameHandling);

        var utf8WithoutBomEncoding = new UTF8Encoding(false);
        var serialized = utf8WithoutBomEncoding
            .GetPreamble()
            .Concat(utf8WithoutBomEncoding.GetBytes($"{{\"{nameof(SimpleMessage.SomeProperty)}\":\"John\"}}"))
            .ToArray();

        using (var stream = new MemoryStream(serialized))
        {
            stream.Position = 0;

            var result = (SimpleMessage)serializer.Deserialize(stream, new[] { typeof(SimpleMessage) })[0];

            Assert.AreEqual("John", result.SomeProperty);
        }
    }

    public class SimpleMessage
    {
        public string SomeProperty { get; set; }
    }

}