using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NServiceBus.Core.Tests;
using NUnit.Framework;
#if NET452
using PublicApiGenerator;

[TestFixture]
public class APIApprovals
{

    [Test]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void Approve()
    {
        var combine = Path.Combine(TestContext.CurrentContext.TestDirectory, "NServiceBus.Newtonsoft.Json.dll");
        var assembly = Assembly.LoadFile(combine);
        var publicApi = Filter(ApiGenerator.GeneratePublicApi(assembly));
        TestApprover.Verify(publicApi);
    }

    string Filter(string text)
    {
        return string.Join(Environment.NewLine, text.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.RemoveEmptyEntries)
            .Where(l => !l.StartsWith("[assembly: ReleaseDateAttribute("))
            .Where(l => !string.IsNullOrWhiteSpace(l))
        );
    }
}
#endif