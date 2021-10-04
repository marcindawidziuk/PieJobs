using PieJobs.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NJsonSchema.Generation;
using PieJobs.Api.Authentication;
using PieJobs.Data;

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
            services.AddSpaStaticFiles(configuration: options => { options.RootPath = "wwwroot"; });
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "PieJobs.Api", Version = "v1" }); });

            var databasePath = Configuration["DatabasePath"];

            services
                .AddDbContextFactory<ApplicationDbContext>(a => a.UseSqlite($"Data Source={databasePath}"));
            
            services
                .AddAuthentication(ApiAuthenticationSchemeOptions.DefaultSchemeName)
                .AddScheme<ApiAuthenticationSchemeOptions, ApiAuthenticationHandler>(
                    ApiAuthenticationSchemeOptions.DefaultSchemeName, 
                    null);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IContextFactory, ContextFactory>();
            services.AddScoped<IJobsService, JobsService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IJobDefinitionService, JobDefinitionService>();
            services.AddScoped<IUsersService, UsersService>();
            
            services.AddOpenApiDocument(document =>
            {
                document.Description = "PieJobs Api";
                document.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.Null;
                document.DefaultResponseReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
            });
            
            services.AddHostedService<JobExecutorHostedService>();
            services.AddSpaStaticFiles(options => { options.RootPath = "dist";});
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
            
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UseSpaStaticFiles();
            app.UseSpa(_ => {});

            var scope = app.ApplicationServices.CreateScope();
            var contextFactory = scope.ServiceProvider.GetRequiredService<IContextFactory>();
            var db = contextFactory.Create();
            db.Database.Migrate();

        }
    }
}