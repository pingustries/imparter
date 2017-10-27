﻿namespace Imparter.Store
{
    public abstract class ChannelFactoryBase : IChannelFactory
    {
        protected abstract IMessageQueue Get(string name);

        public ImparterChannel GetImparterChannel(string channelName)
        {
            return new ImparterChannel(Get(channelName));
        }

        public SubscriberChannel GetSubscriberChannel(string channelName)
        {
            return new SubscriberChannel(Get(channelName));
        }
    }
}