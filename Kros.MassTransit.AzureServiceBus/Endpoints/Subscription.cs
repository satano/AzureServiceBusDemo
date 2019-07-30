using Kros.Utils;
using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using MassTransit.ConsumeConfigurators;
using System;
using System.Collections.Generic;

namespace Kros.MassTransit.AzureServiceBus.Endpoints
{
    /// <summary>
    /// Azure service bus subscription endpoint.
    /// </summary>
    public class SubscriptionEndpoint<TMessage> : Endpoint where TMessage : class
    {
        /// <summary>
        /// Delegate to configure endpoint.
        /// </summary>
        private Action<IServiceBusSubscriptionEndpointConfigurator> _configurator;
        /// <summary>
        /// Delegates to configure individual endpoint consumers.
        /// </summary>
        private List<Action<IServiceBusSubscriptionEndpointConfigurator>> _consumers;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="subscriptionName">Name of subscription.</param>
        /// <param name="configurator">Delegate to configure endpoint.</param>
        public SubscriptionEndpoint(string subscriptionName, Action<IServiceBusSubscriptionEndpointConfigurator> configurator)
        {
            _name = Check.NotNullOrWhiteSpace(subscriptionName, nameof(subscriptionName));
            _configurator = configurator;
            _consumers = new List<Action<IServiceBusSubscriptionEndpointConfigurator>>();
        }

        /// <inheritdoc />
        public override void AddConsumer<TMessage2>(MessageHandler<TMessage2> handler)
            => _consumers.Add(endpointConfig => endpointConfig.Handler(handler));

        /// <inheritdoc />
        public override void AddConsumer<TConsumer>(Action<IConsumerConfigurator<TConsumer>> configure = null)
            => _consumers.Add(endpointConfig => endpointConfig.Consumer(configure));

        /// <inheritdoc />
        public override void SetEndpoint(IServiceBusBusFactoryConfigurator busCfg, IServiceBusHost host)
            => busCfg.SubscriptionEndpoint<TMessage>(host, _name, endpointConfig =>
            {
                _configurator?.Invoke(endpointConfig);

                foreach (Action<IServiceBusSubscriptionEndpointConfigurator> consumerConfig in _consumers)
                {
                    consumerConfig?.Invoke(endpointConfig);
                }
            });
    }
}
