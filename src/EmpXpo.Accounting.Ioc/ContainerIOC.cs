using EmpXpo.Accounting.Application;
using EmpXpo.Accounting.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmpXpo.Accounting.Ioc
{
    public static class ContainerIoc
    {   
        public static IServiceCollection AddContainerIoc(
                this IServiceCollection services,
                IConfiguration configuration
            )
        {
            services.AddApplication()
                    .AddRepository(options =>
                    {
                        options.ConnectionString = configuration["ConnectionStrings:cnSqlCacheFlow"] ?? "";
                    });


            return services;
        }
        
    }
}
