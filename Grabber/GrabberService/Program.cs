using GrabberService.Dao;
using GrabberService.Dao.Interfaces;
using GrabberService.Properties;
using GrabberService.Service;
using GrabberService.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GrabberService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    
                    services.AddTransient<IHttpClient>(_ => new HttpClient(configuration.Get<AppSettings>()));
                    services.AddTransient<IGismeteoGetter>(provider => new GismeteoGetter(provider.GetService<IHttpClient>()));
                    services.AddTransient<IGismeteoParser>(provider => new GismeteoParser(provider.GetService<IGismeteoGetter>()));
                    services.AddHostedService<Worker>();
                });
    }
}