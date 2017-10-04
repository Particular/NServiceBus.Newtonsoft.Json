namespace NServiceBus.Core.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using ApprovalTests;
    using ApprovalTests.Namers;
    using global::Newtonsoft.Json;
    using global::Newtonsoft.Json.Converters;

    static class TestApprover
    {
        public static void Verify(string text)
        {
            var writer = new ApprovalTextWriter(text);
            var namer = new ApprovalNamer();
            Approvals.Verify(writer, namer, Approvals.GetReporter());
        }

        class ApprovalNamer : UnitTestFrameworkNamer
        {
            public ApprovalNamer()
            {
                var assemblyPath = GetType().Assembly.Location;
                var assemblyDir = Path.GetDirectoryName(assemblyPath);

                sourcePath = Path.Combine(assemblyDir, "..", "..", "..", "ApprovalFiles");
            }

            public override string SourcePath => sourcePath;

            readonly string sourcePath;
        }
    }

    public static class ObjectApprover
    {
        static JsonSerializer JsonSerializer;

        static ObjectApprover()
        {
            JsonSerializer = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };
            JsonSerializer.Converters.Add(new StringEnumConverter());
        }

        public static void VerifyWithJson(object target)
        {
            VerifyWithJson(target, s => s);
        }

        public static void VerifyWithJson(object target, Func<string, string> scrubber)
        {
            var formatJson = AsFormattedJson(target);
            TestApprover.Verify(formatJson);
        }

        public static string AsFormattedJson(object target)
        {
            var stringBuilder = new StringBuilder();
            using (var stringWriter = new StringWriter(stringBuilder))
            {
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    jsonWriter.Formatting = JsonSerializer.Formatting;
                    JsonSerializer.Serialize(jsonWriter, target);
                }
                return stringWriter.ToString();
            }
        }
    }
}