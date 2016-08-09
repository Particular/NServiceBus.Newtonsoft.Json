using System.IO;
using NServiceBus;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class Message_with_concrete_implementation_and_interface
{

    [Test]
    public void Run()
    {
        var map = new[]
        {
            typeof(SuperMessageWithConcreteImpl),
            typeof(ISuperMessageWithConcreteImpl)
        };
        var messageMapper = new MessageMapper();
        messageMapper.Initialize(map);
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null);

        using (var stream = new MemoryStream())
        {
            var msg = new SuperMessageWithConcreteImpl
            {
                SomeProperty = "test"
            };

            serializer.Serialize(msg, stream);

            stream.Position = 0;

            var result = (ISuperMessageWithConcreteImpl) serializer.Deserialize(stream, map)[0];

            Assert.IsInstanceOf<SuperMessageWithConcreteImpl>(result);
            Assert.AreEqual("test", result.SomeProperty);
        }
    }

    public interface ISuperMessageWithConcreteImpl : IMyEvent
    {
        string SomeProperty { get; set; }
    }
    public interface IMyEvent : IEvent
    {
    }

    public class SuperMessageWithConcreteImpl : ISuperMessageWithConcreteImpl
    {
        public string SomeProperty { get; set; }
    }
}
