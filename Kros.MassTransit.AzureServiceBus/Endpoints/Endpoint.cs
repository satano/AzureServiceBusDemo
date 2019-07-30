using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using MassTransit.ConsumeConfigurators;
using System;

namespace Kros.MassTransit.AzureServiceBus.Endpoints
{
    /// <summary>
    /// Class representing Azure service bus endpoint.
    /// </summary>
    public abstract class Endpoint
    {
        /// <summary>
        /// Endpoint name.
        /// </summary>
        protected string _name;

        /// <summary>
        /// Adds new consumer to endpoint.
        /// </summary>
        /// <typeparam name="TMessage">Type of message processed by consumer.</typeparam>
        /// <param name="handler">Delegate to process message.</param>
        public abstract void AddConsumer<TMessage>(MessageHandler<TMessage> handler) where TMessage : class;

        /// <summary>
        /// Adds new consumer to endpoint.
        /// </summary>
        /// <typeparam name="TConsumer">Type of message consumer.</typeparam>
        /// <param name="configure">Delegate to configure consumer.</param>
        /// <param name="consumer">Consumer to process message.</param>
        public abstract void AddConsumer<TConsumer>(Action<IConsumerConfigurator<TConsumer>> configure = null)
            where TConsumer : class, IConsumer, new();

        /// <summary>
        /// Sets endpoint and its consumers during service bus initialization.
        /// </summary>
        /// <param name="busCfg">Service bus configuration.</param>
        /// <param name="host">Service bus host.</param>
        public abstract void SetEndpoint(IServiceBusBusFactoryConfigurator busCfg, IServiceBusHost host);
    }
}
