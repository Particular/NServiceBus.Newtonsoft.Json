using System.IO;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class Message_without_wrapping
{

    [Test]
    public void Serialize()
    {
        var messageMapper = new MessageMapper();
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null);
        using (var stream = new MemoryStream())
        {
            serializer.Serialize(new SimpleMessage(), stream);

            stream.Position = 0;
            var result = new StreamReader(stream).ReadToEnd();

            Assert.That(!result.StartsWith("["), result);
        }
    }

    [Test]
    public void Deserialize()
    {
        var messageMapper = new MessageMapper();
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null);
        using (var stream = new MemoryStream())
        {
            serializer.Serialize(new SimpleMessage
            {
                SomeProperty = "test"
            }, stream);

            stream.Position = 0;
            var result = (SimpleMessage) serializer.Deserialize(stream, new[]
            {
                typeof(SimpleMessage)
            })[0];

            Assert.AreEqual("test", result.SomeProperty);
        }

    }
    public class SimpleMessage
    {
        public string SomeProperty { get; set; }
    }
}