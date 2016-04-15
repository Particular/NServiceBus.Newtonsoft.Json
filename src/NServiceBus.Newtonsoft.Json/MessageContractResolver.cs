using System;
using Newtonsoft.Json.Serialization;
using NServiceBus.MessageInterfaces;

namespace NServiceBus.Newtonsoft.Json
{
    using System.Collections.Concurrent;

    class MessageContractResolver : DefaultContractResolver
    {
        IMessageMapper messageMapper;
        ConcurrentDictionary<RuntimeTypeHandle, JsonObjectContract> cache = new ConcurrentDictionary<RuntimeTypeHandle, JsonObjectContract>();

        public MessageContractResolver(IMessageMapper messageMapper)
        {
            this.messageMapper = messageMapper;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            return cache.GetOrAdd(objectType.TypeHandle, handle => BuildJsonObjectContract(objectType));
        }

        JsonObjectContract BuildJsonObjectContract(Type objectType)
        {
            var mappedTypeFor = messageMapper.GetMappedTypeFor(objectType);

            if (mappedTypeFor == null)
            {
                return base.CreateObjectContract(objectType);
            }

            var jsonContract = base.CreateObjectContract(mappedTypeFor);
            jsonContract.DefaultCreator = () => messageMapper.CreateInstance(mappedTypeFor);

            return jsonContract;
        }
    }
}