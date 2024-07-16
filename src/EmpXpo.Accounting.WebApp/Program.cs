using EmpXpo.Accounting.WebApp.Extensions;
using EmpXpo.Accounting.Ioc;

namespace EmpXpo.Accounting.WebApp
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

            builder.Services.AddControllersWithViews();
            builder.Services.AddWebApp(options =>
            {
                options.CashFlowReportApi = builder.Configuration?.GetValue<string>("cnCashFlowReportApi") ?? "";
                options.CashFlowApi = builder.Configuration?.GetValue<string>("cnCashFlowApi") ?? "";
            });
            builder.Services.AddContainerIoc(builder.Configuration);

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection()
               .UseStaticFiles()
               .UseRouting()
               .UseAuthorization();


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
