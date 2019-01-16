using System.IO;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;
using Particular.Approvals;

[TestFixture]
public class Without_wrapping
{
    [Test]
    public void Serialize()
    {
        var messageMapper = new MessageMapper();
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null);
        var message = new SimpleMessage();
        using (var stream = new MemoryStream())
        {
            serializer.Serialize(message, stream);

            stream.Position = 0;
            var result = new StreamReader(stream).ReadToEnd();
            Approver.Verify(result);
        }
    }

    [Test]
    public void Deserialize()
    {
        var messageMapper = new MessageMapper();
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null);
        var messageTypes = new[]
        {
            typeof(SimpleMessage)
        };
        var message = new SimpleMessage
        {
            SomeProperty = "test"
        };
        using (var stream = new MemoryStream())
        {
            serializer.Serialize(message, stream);

            stream.Position = 0;
            var result = (SimpleMessage) serializer.Deserialize(stream, messageTypes)[0];

            Assert.AreEqual("test", result.SomeProperty);
        }

    }

    public class SimpleMessage
    {
        public string SomeProperty { get; set; }
    }
}