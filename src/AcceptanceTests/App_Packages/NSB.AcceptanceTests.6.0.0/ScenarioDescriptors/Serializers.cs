namespace NServiceBus.AcceptanceTests.ScenarioDescriptors
{
    using AcceptanceTesting.Support;

    public static class Serializers
    {

        public static RunDescriptor Json
        {
            get
            {
                var json = new RunDescriptor("Json");
                json.Settings.Set("Serializer", typeof(NewtonsoftSerializer));
                return json;
            }
        }
    }
}