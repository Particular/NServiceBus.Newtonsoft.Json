using System.IO;
using NServiceBus;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class With_concrete_implementation_and_interface
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

        var message = new SuperMessageWithConcreteImpl
        {
            SomeProperty = "test"
        };

        using (var stream = new MemoryStream())
        {
            serializer.Serialize(message, stream);

            stream.Position = 0;

            var result = (ISuperMessageWithConcreteImpl)serializer.Deserialize(stream, map)[0];

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