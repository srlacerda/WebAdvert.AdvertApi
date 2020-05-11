using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AutoMapper;
using AdvertApi.Services;
using AdvertApi.HealthChecks;

namespace AdvertApi
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
            //services.AddAutoMapper();
            services.AddAutoMapper(typeof(AdvertProfile));
            services.AddTransient<IAdvertStorageService, DynamoDBAdvertStorage>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //services.AddHealthChecks();
            services.AddHealthChecks().AddCheck<StorageHealthCheck>("Storage");
            //services.AddHealthChecks(checks =>
            //{
            //    checks.AddCheck<StorageHealthCheck>("Storage", new System.TimeSpan(0, 1, 0));
            //});

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHealthChecks("/health");

            app.UseMvc();
        }
    }
}
