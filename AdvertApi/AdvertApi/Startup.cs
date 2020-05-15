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
using Microsoft.OpenApi.Models;
using Amazon.Util;
using Amazon.ServiceDiscovery;
using Amazon.ServiceDiscovery.Model;

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

            services.AddCors(options =>
            {
                options.AddPolicy("AllOrigin", policy => policy.WithOrigins("*").AllowAnyHeader());
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Web Advertisement Apis",
                    Version = "version 1",
                    Contact = new OpenApiContact
                    {
                        Name = "Diego Lacerda",
                        Email = "diego.lacerda.alves@gmail.com"
                    }
                });
            });

            //services.UseCustomCloudMapClient<CloudMapClient>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        //public async Task Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHealthChecks("/health");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web Advert Api");
            });

            //funcionando HARDCODE
            //RegisterToCloudMap();

            app.UseMvc();
        }

        private async Task RegisterToCloudMap()
        {
            const string serviceId = "srv-mddqavg6dnwc4x2l";
            //var instanceID = EC2InstanceMetadata.InstanceId;
            var instanceID = "i-03277659830d791cf";

            if (!string.IsNullOrEmpty(instanceID))
            {
                //var ipv4 = EC2InstanceMetadata.PrivateIpAddress;
                var ipv4 = "18.218.240.116";
                var client = new AmazonServiceDiscoveryClient();
                await client.RegisterInstanceAsync(new RegisterInstanceRequest
                {
                    InstanceId  = instanceID,
                    ServiceId = serviceId,
                    Attributes = new Dictionary<string, string>() { {"AWS_INSTANCE_IPV4",ipv4 },
                        { "AWS_INSTANCE_PORT","80"} }
                }).ConfigureAwait(false);
            }
        }
    }
}
