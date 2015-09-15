using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NServiceBus;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NUnit.Framework;

[TestFixture]
public class JsonMessageSerializerTest 
{
    public JsonMessageSerializerTest()
    {
        messageMapper = new MessageMapper();
        messageMapper.Initialize(new[]
                                    {
                                        typeof (IA), typeof (A), typeof (SimpleMessage)
                                    });
    }

    JsonMessageSerializer serializer;
    MessageMapper messageMapper;

    [SetUp]
    public void Setup()
    {
        serializer = new JsonMessageSerializer(messageMapper);
    }

    public class SimpleMessage1
    {
        public string PropertyOnMessage1 { get; set; }
    }

    public class SimpleMessage2
    {
        public string PropertyOnMessage2 { get; set; }
    }


    [Test]
    public void Deserialize_messages_wrapped_in_array_from_older_endpoint()
    {
        var jsonWithMultipleMessages = @"
[
{
$type: 'JsonMessageSerializerTest+SimpleMessage1, NServiceBus.Core.Tests',
PropertyOnMessage1: 'Message1'
},
{
$type: 'JsonMessageSerializerTest+SimpleMessage2, NServiceBus.Core.Tests',
PropertyOnMessage2: 'Message2'
}
]";
        using (var stream = new MemoryStream())
        {
            var streamWriter = new StreamWriter(stream);
            streamWriter.Write(jsonWithMultipleMessages);
            streamWriter.Flush();
            stream.Position = 0;
            var result = serializer.Deserialize(stream, new[]
            {
                typeof(SimpleMessage2),
                typeof(SimpleMessage1)
            });

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("Message1", ((SimpleMessage1)result[0]).PropertyOnMessage1);
            Assert.AreEqual("Message2", ((SimpleMessage2)result[1]).PropertyOnMessage2);
        }
    }

    [Test]
    public void Deserialize_message_with_interface_without_wrapping()
    {
        using (var stream = new MemoryStream())
        {
            serializer.Serialize(new SuperMessage { SomeProperty = "John" }, stream);

            stream.Position = 0;

            var result = (SuperMessage)serializer.Deserialize(stream, new[] { typeof(SuperMessage), typeof(IMyEvent) })[0];

            Assert.AreEqual("John", result.SomeProperty);
        }
    }

    [Test]
    public void Deserialize_private_message_with_two_unrelated_interface_without_wrapping()
    {
        messageMapper = new MessageMapper();
        messageMapper.Initialize(new[] { typeof(IMyEventA), typeof(IMyEventB) });
        serializer = new JsonMessageSerializer(messageMapper);

        using (var stream = new MemoryStream())
        {
            var msg = new CompositeMessage
                        {
                IntValue = 42,
                StringValue = "Answer"
            };

            serializer.Serialize(msg, stream);

            stream.Position = 0;

            var result = serializer.Deserialize(stream, new[] { typeof(IMyEventA), typeof(IMyEventB) });
            var a = (IMyEventA)result[0];
            var b = (IMyEventB)result[1];
            Assert.AreEqual(42, b.IntValue);
            Assert.AreEqual("Answer", a.StringValue);
        }
    }

    [Test]
    public void Serialize_message_without_wrapping()
    {
        using (var stream = new MemoryStream())
        {
            serializer.Serialize(new SimpleMessage(), stream);

            stream.Position = 0;
            var result = new StreamReader(stream).ReadToEnd();

            Assert.That(!result.StartsWith("["), result);
        }
    }

    [Test]
    public void Deserialize_message_without_wrapping()
    {
        using (var stream = new MemoryStream())
        {
            serializer.Serialize(new SimpleMessage { SomeProperty = "test" }, stream);

            stream.Position = 0;
            var result = (SimpleMessage)serializer.Deserialize(stream, new[] { typeof(SimpleMessage) })[0];

            Assert.AreEqual("test", result.SomeProperty);
        }

    }

    [Test]
    public void Serialize_message_without_typeInfo()
    {
        using (var stream = new MemoryStream())
        {
            serializer.Serialize(new SimpleMessage(), stream);

            stream.Position = 0;
            var result = new StreamReader(stream).ReadToEnd();

            Assert.That(!result.Contains("$type"), result);
        }
    }

    [Test]
    public void Serialize_message_without_concrete_implementation()
    {
        messageMapper = new MessageMapper();
        messageMapper.Initialize(new[] { typeof(ISuperMessageWithoutConcreteImpl) });
        serializer = new JsonMessageSerializer(messageMapper);

        using (var stream = new MemoryStream())
        {
            serializer.Serialize(messageMapper.CreateInstance<ISuperMessageWithoutConcreteImpl>(), stream);

            stream.Position = 0;
            var result = new StreamReader(stream).ReadToEnd();

            Assert.That(!result.Contains("$type"), result);
            Assert.That(result.Contains("SomeProperty"), result);
        }
    }

    [Test]
    public void Deserialize_message_without_concrete_implementation()
    {
        messageMapper = new MessageMapper();
        messageMapper.Initialize(new[] { typeof(ISuperMessageWithoutConcreteImpl) });
        serializer = new JsonMessageSerializer(messageMapper);

        using (var stream = new MemoryStream())
        {
            var msg = messageMapper.CreateInstance<ISuperMessageWithoutConcreteImpl>();
            msg.SomeProperty = "test";

            serializer.Serialize(msg, stream);

            stream.Position = 0;

            var result = (ISuperMessageWithoutConcreteImpl)serializer.Deserialize(stream, new[] { typeof(ISuperMessageWithoutConcreteImpl) })[0];

            Assert.AreEqual("test", result.SomeProperty);
        }
    }

    [Test]
    public void Deserialize_message_with_concrete_implementation_and_interface()
    {
        var map = new[] { typeof(SuperMessageWithConcreteImpl), typeof(ISuperMessageWithConcreteImpl) };
        messageMapper = new MessageMapper();
        messageMapper.Initialize(map);
        serializer = new JsonMessageSerializer(messageMapper);

        using (var stream = new MemoryStream())
        {
            var msg = new SuperMessageWithConcreteImpl
            {
                SomeProperty = "test"
            };

            serializer.Serialize(msg, stream);

            stream.Position = 0;

            var result = (ISuperMessageWithConcreteImpl)serializer.Deserialize(stream, map)[0];

            Assert.IsInstanceOf<SuperMessageWithConcreteImpl>(result);
            Assert.AreEqual("test", result.SomeProperty);
        }
    }



    [Test]
    public void TestMany()
    {
        var xml = @"[{
$type: ""IA, NServiceBus.Core.Tests"",
Data: ""rhNAGU4dr/Qjz6ocAsOs3wk3ZmxHMOg="",
S: ""kalle"",
I: 42,
B: {
    BString: ""BOO"",
    C: {
        $type: ""C, NServiceBus.Core.Tests"",
        Cstr: ""COO""
    }
}
}, {
$type: ""IA, NServiceBus.Core.Tests"",
Data: ""AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA="",
S: ""kalle"",
I: 42,
B: {
    BString: ""BOO"",
    C: {
        $type: ""C, NServiceBus.Core.Tests"",
        Cstr: ""COO""
    }
}
}]";
        using (var stream = new MemoryStream())
        {
            var streamWriter = new StreamWriter(stream);
            streamWriter.Write(xml);
            streamWriter.Flush();
            stream.Position = 0;

            messageMapper = new MessageMapper();
            messageMapper.Initialize(new[] { typeof(IAImpl), typeof(IA) });
            serializer = new JsonMessageSerializer(messageMapper);

            var result = serializer.Deserialize(stream, new[] { typeof(IAImpl) });
            Assert.IsNotEmpty(result);
            Assert.That(result, Has.Length.EqualTo(2));

            Assert.That(result[0], Is.AssignableTo(typeof(IA)));
            var a = ((IA)result[0]);

            Assert.AreEqual(23, a.Data.Length);
            Assert.AreEqual(42, a.I);
            Assert.AreEqual("kalle", a.S);
            Assert.IsNotNull(a.B);
            Assert.AreEqual("BOO", a.B.BString);
            Assert.AreEqual("COO", ((C)a.B.C).Cstr);
        }

    }

    [Test]
    public void Test()
    {
        var expectedDate = new DateTime(2010, 10, 13, 12, 32, 42, DateTimeKind.Unspecified);
        var expectedDateLocal = new DateTime(2010, 10, 13, 12, 32, 42, DateTimeKind.Local);
        var expectedDateUtc = new DateTime(2010, 10, 13, 12, 32, 42, DateTimeKind.Utc);
        var expectedGuid = Guid.NewGuid();

        var obj = new A
                    {
                        AGuid = expectedGuid,
                        Data = new byte[32],
                        I = 23,
                        S = "Foo",
                        Ints = new List<int>
                                {
                                    12, 42
                                },
                        Bs = new List<B>
                            {
                                new B
                                {
                                    BString = "aaa", C = new C
                                                        {
                                                            Cstr = "ccc"
                                                        }
                                },
                                new BB
                                {
                                    BString = "bbbb", C = new C
                                                            {
                                                                Cstr = "dddd"
                                                            },
                                    BBString = "BBStr"
                                }
                            },
                        DateTime = expectedDate,
                        DateTimeLocal = expectedDateLocal,
                        DateTimeUtc = expectedDateUtc
                    };

        new Random().NextBytes(obj.Data);

        var output = new MemoryStream();

        messageMapper = new MessageMapper();
        messageMapper.Initialize(new[]
                                    {
                                        typeof (IA), typeof (A)
                                    });
        serializer = new JsonMessageSerializer(messageMapper);

        serializer.Serialize(obj, output);

        output.Position = 0;

        var result = serializer.Deserialize(output, new[]
                                                    {
                                                        typeof (A)
                                                    });

        Assert.DoesNotThrow(() => output.Position = 0, "Stream should still be open");

        Assert.That(result[0], Is.TypeOf(typeof (A)));
        var a = ((A) result[0]);

        Assert.AreEqual(obj.Data, a.Data);
        Assert.AreEqual(23, a.I);
        Assert.AreEqual("Foo", a.S);
        Assert.AreEqual(expectedDate.Kind, a.DateTime.Kind);
        Assert.AreEqual(expectedDate, a.DateTime);
        Assert.AreEqual(expectedDateLocal.Kind, a.DateTimeLocal.Kind);
        Assert.AreEqual(expectedDateLocal, a.DateTimeLocal);
        Assert.AreEqual(expectedDateUtc.Kind, a.DateTimeUtc.Kind);
        Assert.AreEqual(expectedDateUtc, a.DateTimeUtc);
        Assert.AreEqual("ccc", ((C) a.Bs[0].C).Cstr);
        Assert.AreEqual(expectedGuid, a.AGuid);

        Assert.IsInstanceOf<B>(a.Bs[0]);
        Assert.IsInstanceOf<BB>(a.Bs[1]);
    }

    [Test]
    public void TestInterfaces()
    {
        var output = new MemoryStream();

        var obj = messageMapper.CreateInstance<IA>(
            x =>
            {
                x.S = "kalle";
                x.I = 42;
                x.Data = new byte[23];
                x.B = new B
                        {
                            BString = "BOO", C = new C
                                                {
                                                    Cstr = "COO"
                                                }
                        };
            }
            );

        new Random().NextBytes(obj.Data);

        messageMapper = new MessageMapper();
        messageMapper.Initialize(new[]
                                    {
                                        typeof (IA), typeof (IAImpl)
                                    });
        serializer = new JsonMessageSerializer(messageMapper);

        serializer.Serialize(obj, output);

        output.Position = 0;

        var filename = string.Format("{0}.{1}.txt", GetType().Name, MethodBase.GetCurrentMethod().Name);

        File.WriteAllBytes(filename, output.ToArray());

        output.Position = 0;

        var result = serializer.Deserialize(output, new[]
                                                    {
                                                        typeof (IAImpl)
                                                    });

        Assert.DoesNotThrow(() => output.Position = 0, "Stream should still be open");

        Assert.IsNotEmpty(result);
        Assert.That(result, Has.Length.EqualTo(1));

        Assert.That(result[0], Is.AssignableTo(typeof (IA)));
        var a = ((IA) result[0]);

        Assert.AreEqual(a.Data, obj.Data);
        Assert.AreEqual(42, a.I);
        Assert.AreEqual("kalle", a.S);
        Assert.IsNotNull(a.B);
        Assert.AreEqual("BOO", a.B.BString);
        Assert.AreEqual("COO", ((C) a.B.C).Cstr);
    }
}

public class SimpleMessage
{
    public string SomeProperty { get; set; }
}
public class SuperMessage : IMyEvent
{
    public string SomeProperty { get; set; }
}

public interface IMyEvent
{
}

public class CompositeMessage : IMyEventA, IMyEventB
{
    public string StringValue { get; set; }
    public int IntValue { get; set; }
}

public interface IMyEventA
{
    string StringValue { get; set; }
}

public class MyEventA_impl : IMyEventA
{
    public string StringValue { get; set; }
}

public interface IMyEventB
{
    int IntValue { get; set; }
}

public class MyEventB_impl : IMyEventB
{
    public int IntValue { get; set; }
}

public interface ISuperMessageWithoutConcreteImpl : IMyEvent
{
    string SomeProperty { get; set; }
}

public interface ISuperMessageWithConcreteImpl : IMyEvent
{
    string SomeProperty { get; set; }
}

public class SuperMessageWithConcreteImpl : ISuperMessageWithConcreteImpl
{
    public string SomeProperty { get; set; }
}

public class A : IMessage
{
    public Guid AGuid { get; set; }
    public byte[] Data;
    public string S;
    public int I { get; set; }

    public DateTime DateTime { get; set; }
    public DateTime DateTimeLocal { get; set; }
    public DateTime DateTimeUtc { get; set; }

    public List<int> Ints { get; set; }
    public List<B> Bs { get; set; }

}

public interface IA : IMessage
{
    byte[] Data { get; set; }
    string S { get; set; }
    int I { get; set; }
    B B { get; set; }
}
public class IAImpl : IA
{
    public byte[] Data { get; set; }
    public string S { get; set; }
    public int I { get; set; }
    public B B { get; set; }
}

public class B
{
    public string BString { get; set; }
    public object C { get; set; }
}

public class BB : B
{
    public string BBString { get; set; }
}

public class C
{
    public string Cstr { get; set; }
}



