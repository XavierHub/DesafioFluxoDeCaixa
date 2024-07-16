using AutoMapper;
using EmpXpo.Accounting.Domain;
using EmpXpo.Accounting.Domain.Abstractions.Application;
using EmpXpo.Accounting.Domain.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace EmpXpo.Accounting.Application
{
    public static class ApplicationExtension
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICashFlowApplication, CashFlowApplication>();
            services.AddScoped<ICashFlowReportApplication, CashFlowReportApplication>();

            var cfg = new MapperConfigurationExpression();
            cfg.CreateMap<CashFlow, CashFlowViewModel>().ReverseMap();

            var mapperConfig = new MapperConfiguration(cfg);
            services.AddSingleton<IMapper, Mapper>(x => new Mapper(mapperConfig));

            return services;
        }
    }
}
