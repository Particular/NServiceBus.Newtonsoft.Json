using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Newtonsoft.Json;
using NUnit.Framework;
using Particular.Approvals;

[TestFixture]
public class ArrayTests
{
    string typeName = $"{typeof(ArrayMessage).FullName}, {typeof(ArrayMessage).Assembly.GetName().Name}";

    [Test]
    public void Should_throw_for_multiple_dollar()
    {
        var text = $@"
[{{
    $type: '{typeName}',
    SomeProperty: 'Value1'
}},
{{
    $type: '{typeName}',
    SomeProperty: 'Value2'
}}]";

        using (var stream = new MemoryStream())
        {
            WriteToStream(stream, text);

            var messageMapper = new MessageMapper();
            var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null);
            var exception = Assert.Throws<Exception>(() => serializer.Deserialize(stream, new List<Type>()));
            Approver.Verify(exception.Message);
        }
    }

    [Test]
    public void Should_throw_for_multiple_passed_type()
    {
        var text = @"
[{
    SomeProperty: 'Value1'
},
{
    SomeProperty: 'Value2'
}]";

        using (var stream = new MemoryStream())
        {
            WriteToStream(stream, text);

            var messageMapper = new MessageMapper();
            var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null);
            var messageTypes = new List<Type>
            {
                typeof(ArrayMessage)
            };
            var exception = Assert.Throws<Exception>(() =>
            {
                serializer.Deserialize(stream, messageTypes);
            });
            Approver.Verify(exception.Message);
        }
    }

    [Test]
    public void Should_not_throw_for_single_dollar()
    {
        var text = $@"
[{{
    $type: '{typeName}',
    SomeProperty: 'Value1'
}}]";

        using (var stream = new MemoryStream())
        {
            WriteToStream(stream, text);

            var messageMapper = new MessageMapper();
            var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null);
            var deserialize = serializer.Deserialize(stream, new List<Type>());
            Approver.Verify(deserialize.Single());
        }
    }


    [Test]
    public void Should_not_throw_for_single_passed_type()
    {
        var text = @"
[{
    SomeProperty: 'Value1'
}]";

        using (var stream = new MemoryStream())
        {
            WriteToStream(stream, text);

            var messageMapper = new MessageMapper();
            var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null);
            var messageTypes = new List<Type>
            {
                typeof(ArrayMessage)
            };
            var deserialize = serializer.Deserialize(stream, messageTypes);
            Approver.Verify(deserialize.Single());
        }
    }

    static void WriteToStream(MemoryStream stream, string text)
    {
        var streamWriter = new StreamWriter(stream);
        streamWriter.Write(text);
        streamWriter.Flush();
        stream.Position = 0;
    }

    public class ArrayMessage
    {
        public string SomeProperty { get; set; }
    }
}