using Kros.Utils;
using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using MassTransit.ConsumeConfigurators;
using System;
using System.Collections.Generic;

namespace Kros.MassTransit.AzureServiceBus.Endpoints
{
    /// <summary>
    /// Azure service bus receive endpoint.
    /// </summary>
    public class ReceiveEndpoint : Endpoint
    {
        /// <summary>
        /// Delegate to configure endpoint.
        /// </summary>
        private Action<IServiceBusReceiveEndpointConfigurator> _configurator;
        /// <summary>
        /// Delegates to configure individual endpoint consumers.
        /// </summary>
        private List<Action<IServiceBusReceiveEndpointConfigurator>> _consumers;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="queueName">Name of queue.</param>
        /// <param name="configurator">Delegate to configure endpoint.</param>
        public ReceiveEndpoint(string queueName, Action<IServiceBusReceiveEndpointConfigurator> configurator)
        {
            _name = Check.NotNullOrWhiteSpace(queueName, nameof(queueName));
            _configurator = configurator;
            _consumers = new List<Action<IServiceBusReceiveEndpointConfigurator>>();
        }

        /// <inheritdoc />
        public override void AddConsumer<TMessage>(MessageHandler<TMessage> handler)
            => _consumers.Add(endpointConfig => endpointConfig.Handler(handler));

        /// <inheritdoc />
        public override void AddConsumer<TConsumer>(Action<IConsumerConfigurator<TConsumer>> configure = null)
            => _consumers.Add(endpointConfig => endpointConfig.Consumer(configure));

        /// <inheritdoc />
        public override void SetEndpoint(IServiceBusBusFactoryConfigurator busCfg, IServiceBusHost host)
            => busCfg.ReceiveEndpoint(host, _name, endpointConfig =>
            {
                _configurator?.Invoke(endpointConfig);

                foreach (Action<IServiceBusReceiveEndpointConfigurator> consumerConfig in _consumers)
                {
                    consumerConfig?.Invoke(endpointConfig);
                }
            });
    }
}
