using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using ConferenceAPI.Core;
using ConferenceAPI.Data;
using Microsoft.EntityFrameworkCore;
using ConferenceAPI.Core.Services;
using ConferenceAPI.Services;

namespace ConferenceAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ConferenceDbContext>(options => options.UseMySql(Configuration.GetConnectionString("MySQL")));

            services.AddScoped<DbContext>(provider => provider.GetService<ConferenceDbContext>());

            services.AddControllers();

            services.AddAutoMapper(typeof(ConferenceProfile));

            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

            services.AddScoped(typeof(IAvailabilityService), typeof(AvailabilityService));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
