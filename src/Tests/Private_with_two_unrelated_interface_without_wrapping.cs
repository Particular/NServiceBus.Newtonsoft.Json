using System.IO;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class Private_with_two_unrelated_interface_without_wrapping
{

    [Test]
    public void Run()
    {
        var messageMapper = new MessageMapper();
        var messageTypes = new[]
        {
            typeof(IMyEventA),
            typeof(IMyEventB)
        };
        messageMapper.Initialize(messageTypes);
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null);

        var message = new CompositeMessage
        {
            IntValue = 42,
            StringValue = "Answer"
        };

        using (var stream = new MemoryStream())
        {
            serializer.Serialize(message, stream);

            stream.Position = 0;

            var result = serializer.Deserialize(stream, messageTypes);
            var a = (IMyEventA)result[0];
            var b = (IMyEventB)result[1];
            Assert.AreEqual(42, b.IntValue);
            Assert.AreEqual("Answer", a.StringValue);
        }
    }

    class CompositeMessage : IMyEventA, IMyEventB
    {
        public string StringValue { get; set; }
        public int IntValue { get; set; }
    }

    public interface IMyEventA
    {
        string StringValue { get; set; }
    }

    public interface IMyEventB
    {
        int IntValue { get; set; }
    }
}