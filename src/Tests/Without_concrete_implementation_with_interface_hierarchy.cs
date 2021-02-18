using System;
using System.IO;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;

[TestFixture]
public class Without_concrete_implementation_with_interface_hierarchy
{
    [TestCase("Subclass first", typeof(ICustomerChanged), typeof(IItemChanged))]
    [TestCase("Parent class first", typeof(IItemChanged), typeof(ICustomerChanged))]
    public void Order_of_message_types_should_not_matter(string order, Type firstType, Type secondType)
    {
        var messageMapper = new MessageMapper();
        var messageTypes = new[] { firstType, secondType };

        messageMapper.Initialize(messageTypes);
        var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null);

        var message = new CustomerChangedEvent();
        using (var stream = new MemoryStream())
        {
            serializer.Serialize(message, stream);

            stream.Position = 0;

            var result = serializer.Deserialize(stream, messageTypes);

            Assert.AreEqual(1, result.Length, "Should only generate one proxy");
            var proxy = result[0];
            Assert.IsTrue(proxy is ICustomerChanged, "Generated proxy should implement ICustomerChanged");
            Assert.IsTrue(proxy is IItemChanged, "Generated proxy should implement IItemChanged");
        }
    }

    public interface IItemChanged { }
    public interface ICustomerChanged : IItemChanged { }
    public class CustomerChangedEvent : ICustomerChanged { }
}