using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Ares
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                            .UseUrls("http://localhost:5000")
                            .UseStartup<Startup>()
                            .ConfigureServices((services) =>
                            {
                                foreach (var aresService in AresServices())
                                {
                                    services.Add(aresService);
                                }
                            });
        }

        public static IServiceCollection AresServices()
        {
            var services = new ServiceCollection();
            
            services.AddTransient<IRepository<Auction>,Repository<Auction>>();

            return services;
        }
    }
}
