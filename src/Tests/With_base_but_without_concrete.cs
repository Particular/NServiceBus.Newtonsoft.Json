using System.IO;
using System.Linq;
using NServiceBus;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class With_base_type_but_without_sub_type_in_message_types
{
    [Test]
    [TestCase(Newtonsoft.Json.TypeNameHandling.Auto)]
    [TestCase(Newtonsoft.Json.TypeNameHandling.None)]
    public void Deserialize(Newtonsoft.Json.TypeNameHandling typeNameHandling)
    {
        var messageMapper = new MessageMapper();
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null, typeNameHandling);
        var messageTypes = new[]
        {
            typeof(ISomeBaseMessage)
        };
        messageMapper.Initialize(messageTypes.Union(new[]
        {
            typeof(ISomeMessage)
        }));
        var message = messageMapper.CreateInstance<ISomeMessage>(x => x.SomeProperty = "test");
        using (var stream = new MemoryStream())
        {
            serializer.Serialize(message, stream);

            stream.Position = 0;
            var result = (ISomeBaseMessage)serializer.Deserialize(stream, messageTypes)[0];

            Assert.AreEqual("test", result.SomeProperty);
        }

    }

    public interface ISomeMessage : ISomeBaseMessage
    {
    }

    public interface ISomeBaseMessage : IMessage
    {
        string SomeProperty { get; set; }
    }
}