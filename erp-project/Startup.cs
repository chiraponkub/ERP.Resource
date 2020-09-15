using erp_project.Libraries.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using erp_project.Configs;
using erp_project.Middlewares;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace erp_project
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.RegisterLibraries();
            services.AddControllers().AddJsonOptions(m => m.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddAuthenticationHelper();
            services.AddSwaggerGenHelper(Configuration);
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(builder => builder
               .WithOrigins(Configuration.GetValue<string>("ApiUrls:OriginURL", "").Split(",").Select(m => m.Trim()).ToArray())
               .WithMethods("GET", "POST", "PUT", "DELETE", "OPTION")
               .AllowAnyHeader()
               .AllowCredentials());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerHelper();
            }

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
