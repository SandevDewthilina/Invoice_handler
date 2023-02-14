    using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using HRMS_WEB.Models;
using HRMS_WEB.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using AutoMapper;
    using HRMS_WEB.Repositories;

    namespace HRMS_WEB
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
            services.AddControllersWithViews();

            var emailConfig = Configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();

            services.AddSingleton(emailConfig);

            // MySQL DB connection service
            services.AddDbContextPool<HRMSDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            // EFCore identity and set password validations  
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<HRMSDbContext>();

            // Automapper service for DTO's
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Repository dependency injection
            services.AddScoped<IScraper, Scraper>();
            services.AddScoped<IStorageRepository, StorageRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles(new StaticFileOptions
            {

                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "node_modules")),

                RequestPath = "/node_modules"

            });


            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default", 
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
