﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using TheFlightShop.DAL;

namespace TheFlightShop
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var connectionStringTemplate = Configuration.GetConnectionString("FlightShopData");
            var databaseUrl = Environment.GetEnvironmentVariable("CLEARDB_DATABASE_URL");
            var urlParts = databaseUrl.Split('@');
            var credentials = urlParts[0].Split("mysql://")[1].Split(':');
            var username = /**"FlightShopAdmin";*/  credentials[0];
            var password = /**"fly2mySHOP!";*/ credentials[1];
              var urlPath = urlParts[1].Split('/');
            var host = /**"localhost";*/ urlPath[0];
            var schema = /**"FlightShopData";*/ urlPath[1].Split('?')[0];
            var connectionString = string.Format(connectionStringTemplate, host, schema, username, password);
            services.AddScoped<IProductReadDAL>(_ => new ProductReadDAL(connectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCookiePolicy();

            app.UseStaticFiles(); // wwwroot
            // don't be a hero. this worked in dev but not heroku.
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(
            //        Path.Combine(Directory.GetCurrentDirectory(), "product-static-files")
            //    ),
            //    RequestPath = "/products"
            //});

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
