using System;
using Newtonsoft.Json.Serialization;
using NServiceBus.MessageInterfaces;

namespace NServiceBus.Newtonsoft.Json
{

    class MessageContractResolver : DefaultContractResolver
    {
        IMessageMapper messageMapper;

        public MessageContractResolver(IMessageMapper messageMapper)
        {
            this.messageMapper = messageMapper;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
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