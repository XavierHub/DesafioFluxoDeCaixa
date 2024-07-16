using EmpXpo.Accounting.Application.Services;
using EmpXpo.Accounting.Domain.Abstractions.Services;
using EmpXpo.Accounting.Repository;

namespace EmpXpo.Accounting.WebApp.Extensions
{
    public static class WebAppExtension
    {
        public static IServiceCollection AddWebApp(this IServiceCollection services, Action<WebAppOptions> options)
        {

            if (options == null)
                throw new ArgumentNullException(nameof(options), @"Please provide options.");

            services.Configure(options);
            services.AddScoped<IClientApiApplicationService, ClientApiApplicationService>();

            return services;
        }
    }
}
