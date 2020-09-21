using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace erp_project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseStartup<Startup>()
                    .ConfigureKestrel((context, options) =>
                    {
                        // Handle requests up to MaxValue
                        options.Limits.MaxRequestBodySize = 209715200;
                    })
                    .UseIISIntegration();
                });
    }
}
