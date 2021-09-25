using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PieJobs.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NJsonSchema.Generation;

namespace PieJobs.Api
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
            services.AddControllers();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "PieJobs.Api", Version = "v1" }); });

            services.AddScoped<IJobsService, JobsService>();
            
            
            services.AddOpenApiDocument(document =>
            {
                document.Description = "FitTick Api";
                document.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.Null;
                document.DefaultResponseReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
            });
            
            services.AddHostedService<JobExecutorHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
                app.UseCors(
                    options => options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials()
                );
                app.UseDeveloperExceptionPage();
                app.UseOpenApi(p => p.Path = "/swagger/{documentName}/swagger.yaml");
                app.UseSwaggerUi3(p => p.DocumentPath = "/swagger/{documentName}/swagger.yaml");
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}