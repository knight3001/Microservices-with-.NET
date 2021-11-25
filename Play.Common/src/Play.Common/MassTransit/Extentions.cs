using System;
using System.Reflection;
using GreenPipes;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit
{
    public static class Extentions
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
        {
            services.AddMassTransit(configure =>
           {
               configure.AddConsumers(Assembly.GetEntryAssembly());
               configure.UsingRabbitMq((context, configurator) =>
               {
                   var configuration = context.GetService<IConfiguration>();
                   var serviveSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                   var rabbitMqSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                   configurator.Host(rabbitMqSettings.Host);
                   configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviveSettings.ServiceName, false));
                   configurator.UseMessageRetry(retryConfigurator =>
                   {
                       retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
                   });
               });
           });

            services.AddMassTransitHostedService();

            return services;
        }
    }
}