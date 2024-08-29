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
            typeof(SuperMessageWithConcreteImplementation),
            typeof(ISuperMessageWithConcreteImplementation)
        };
        var messageMapper = new MessageMapper();
        messageMapper.Initialize(map);
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null);

        var message = new SuperMessageWithConcreteImplementation
        {
            SomeProperty = "test"
        };

        using (var stream = new MemoryStream())
        {
            serializer.Serialize(message, stream);

            stream.Position = 0;

            var result = (ISuperMessageWithConcreteImplementation)serializer.Deserialize(stream.ToArray(), map)[0];

            Assert.IsInstanceOf<SuperMessageWithConcreteImplementation>(result);
            Assert.That(result.SomeProperty, Is.EqualTo("test"));
        }
    }

    public interface ISuperMessageWithConcreteImplementation : IMyEvent
    {
        string SomeProperty { get; set; }
    }

    public interface IMyEvent : IEvent
    {
    }

    public class SuperMessageWithConcreteImplementation : ISuperMessageWithConcreteImplementation
    {
        public string SomeProperty { get; set; }
    }
}