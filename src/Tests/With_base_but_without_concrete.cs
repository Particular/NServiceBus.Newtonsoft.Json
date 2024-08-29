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
    public void Deserialize()
    {
        var messageMapper = new MessageMapper();
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null);
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
            var result = (ISomeBaseMessage)serializer.Deserialize(stream.ToArray(), messageTypes)[0];

            Assert.That(result.SomeProperty, Is.EqualTo("test"));
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