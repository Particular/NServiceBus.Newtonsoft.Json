using System.IO;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class Message_without_concrete_implementation_and_interface
{

    [Test]
    public void Serialize()
    {
        var messageMapper = new MessageMapper();
        messageMapper.Initialize(new[] { typeof(IWithoutConcrete) });
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null);

        using (var stream = new MemoryStream())
        {
            serializer.Serialize(messageMapper.CreateInstance<IWithoutConcrete>(), stream);

            stream.Position = 0;
            var result = new StreamReader(stream).ReadToEnd();

            Assert.That(!result.Contains("$type"), result);
            Assert.That(result.Contains("SomeProperty"), result);
        }
    }

    [Test]
    public void Deserialize()
    {
        var messageMapper = new MessageMapper();
        messageMapper.Initialize(new[] { typeof(IWithoutConcrete) });
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null);

        using (var stream = new MemoryStream())
        {
            var msg = messageMapper.CreateInstance<IWithoutConcrete>();
            msg.SomeProperty = "test";

            serializer.Serialize(msg, stream);

            stream.Position = 0;

            var result = (IWithoutConcrete)serializer.Deserialize(stream, new[] { typeof(IWithoutConcrete) })[0];

            Assert.AreEqual("test", result.SomeProperty);
        }
    }

    interface IWithoutConcrete
    {
        string SomeProperty { get; set; }
    }
}
