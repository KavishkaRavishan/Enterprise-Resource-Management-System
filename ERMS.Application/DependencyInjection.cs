using ERMS.Application.Common;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ERMS.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            // Register MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            // Register FluentValidation Validators
            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
}
