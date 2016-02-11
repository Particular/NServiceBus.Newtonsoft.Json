using System;
using NServiceBus.Newtonsoft.Json;
using NServiceBus.AcceptanceTests.ScenarioDescriptors;

public static class ModuleInitializer
{
    public static void Initialize()
    {
        var typeName = typeof(NServiceBus.NewtonsoftSerializer).AssemblyQualifiedName;
        Transports.Default.Settings["Serializer"] = typeName;
    }
}