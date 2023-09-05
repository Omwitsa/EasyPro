using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using DinkToPdf;
using DinkToPdf.Contracts;
using EasyPro.IProvider;
using EasyPro.Models;
using EasyPro.Models.BosaModels;
using EasyPro.Provider;
using EasyPro.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;

namespace EasyPro
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
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddDbContext<MORINGAContext>(options => 
            {
                
                options.UseSqlServer(Configuration.GetConnectionString("MoringaDbConnection"));
                options.EnableSensitiveDataLogging(true);
            });

            services.AddDbContext<BosaDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("BosaDbConnection")));
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            services.AddTransient<IReporting, ReportingConcrete>();
            services.AddTransient<IReportProvider, ReportProvider>();
            services.AddNotyf(config => 
            { 
                config.DurationInSeconds = 10; 
                config.IsDismissable = true; 
                config.Position = NotyfPosition.TopCenter; 
            });
            services.AddDistributedMemoryCache();
            services.AddSession(options => {   
                options.IdleTimeout = TimeSpan.FromHours(1);
            });
            services.AddControllersWithViews();
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                // Use the default property (Pascal) casing
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
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

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDM0MjEzQDMxMzkyZTMxMmUzMGtBR1h5Rk56UkRXbXZ6NUFSN2M0OXc0ZXlaSEZFeGlhQlZDdW9hdDZwQUU9;NDM0MjE0QDMxMzkyZTMxMmUzMFptdlZOck5kQlRoa3JubHhkaFNSVGhjTnd3QUhGeXBBZ2tpb21VbEYxRzQ9;NDM0MjE1QDMxMzkyZTMxMmUzMGFQQloveEM4WEpNSWFYYWE3RUtzVGkyRkhJamwvQ2dtcHhRaXlaVmZUV1k9");


            app.UseFastReport();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseAuthorization();
            app.UseNotyf();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Login}/{id?}");
                   // pattern: "{controller=Home}/{action=NewUI}/{id?}");
            });
        }
    }
}
