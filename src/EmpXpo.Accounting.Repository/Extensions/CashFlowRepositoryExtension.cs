using Dommel;
using EmpXpo.Accounting.Domain.Abstractions.Repositories;
using EmpXpo.Accounting.Repository.Conventions;
using Microsoft.Extensions.DependencyInjection;

namespace EmpXpo.Accounting.Repository
{
    public static class CashFlowRepositoryExtension
    {
        public static IServiceCollection AddRepository(
                this IServiceCollection services,
                Action<CashFlowRepositoryOptions> options
            )
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options), @"Please provide options.");

            services.Configure(options);

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            DommelMapper.SetTableNameResolver(new TableNameResolver());

            return services;
        }
    }
}
