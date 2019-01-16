using System.IO;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;
using Particular.Approvals;

[TestFixture]
public class Without_concrete_implementation_and_interface
{
    [Test]
    public void Serialize()
    {
        var messageMapper = new MessageMapper();
        var types = new[]
        {
            typeof(IWithoutConcrete)
        };
        messageMapper.Initialize(types);
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null);

        var message = messageMapper.CreateInstance<IWithoutConcrete>();
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
        var messageTypes = new[]
        {
            typeof(IWithoutConcrete)
        };
        messageMapper.Initialize(messageTypes);
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null);

        var message = messageMapper.CreateInstance<IWithoutConcrete>();
        message.SomeProperty = "test";
        using (var stream = new MemoryStream())
        {
            serializer.Serialize(message, stream);

            stream.Position = 0;

            var result = (IWithoutConcrete) serializer.Deserialize(stream, messageTypes)[0];

            Assert.AreEqual("test", result.SomeProperty);
        }
    }

    public interface IWithoutConcrete
    {
        string SomeProperty { get; set; }
    }
}