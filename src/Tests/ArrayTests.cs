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
    [TestCase(Newtonsoft.Json.TypeNameHandling.Auto)]
    [TestCase(Newtonsoft.Json.TypeNameHandling.None)]
    public void Should_throw_for_multiple_dollar(Newtonsoft.Json.TypeNameHandling typeNameHandling)
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
            var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null, typeNameHandling);
            var exception = Assert.Throws<Exception>(() => serializer.Deserialize(stream, new List<Type>()));
            Approver.Verify(exception.Message);
        }
    }

    [Test]
    [TestCase(Newtonsoft.Json.TypeNameHandling.Auto)]
    [TestCase(Newtonsoft.Json.TypeNameHandling.None)]
    public void Should_throw_for_multiple_passed_type(Newtonsoft.Json.TypeNameHandling typeNameHandling)
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
            var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null, typeNameHandling);
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
    [TestCase(Newtonsoft.Json.TypeNameHandling.Auto)]
    [TestCase(Newtonsoft.Json.TypeNameHandling.None)]
    public void Should_not_throw_for_single_dollar(Newtonsoft.Json.TypeNameHandling typeNameHandling)
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
            var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null, typeNameHandling);
            var deserialize = serializer.Deserialize(stream, new List<Type>());
            Approver.Verify(deserialize.Single());
        }
    }


    [Test]
    [TestCase(Newtonsoft.Json.TypeNameHandling.Auto)]
    [TestCase(Newtonsoft.Json.TypeNameHandling.None)]
    public void Should_not_throw_for_single_passed_type(Newtonsoft.Json.TypeNameHandling typeNameHandling)
    {
        var text = @"
[{
    SomeProperty: 'Value1'
}]";

        using (var stream = new MemoryStream())
        {
            WriteToStream(stream, text);

            var messageMapper = new MessageMapper();
            var serializer = new JsonMessageSerializer(messageMapper, null, null, null, null, typeNameHandling);
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