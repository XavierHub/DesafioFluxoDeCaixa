using EmpXpo.Accounting.Application;
using EmpXpo.Accounting.CashFlowApi.Middlewares;
using EmpXpo.Accounting.Ioc;

namespace EmpXpo.Accounting.CashFlowReportApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                   .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                   .AddEnvironmentVariables()
                   .AddCommandLine(args);

            builder.Services.AddEndpointsApiExplorer()
                            .AddSwaggerGen()
                            .AddContainerIoc(builder.Configuration)
                            //.AddApplication()
                            //.AddRepository(options =>
                            //{
                            //    options.ConnectionString = builder.Configuration?.GetValue<string>("ConnectionStrings:cnSqlCacheFlow") ?? "";
                            //})
                            .AddControllers()
                            .AddJsonOptions(options =>
                                             options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                                            );

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<CashFlowReportExceptionHandlingMiddleware>()
               .UseHttpsRedirection()
               .UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}