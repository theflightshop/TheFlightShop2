using System;
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
using TheFlightShop.Auth;
using TheFlightShop.DAL;
using TheFlightShop.Email;
using TheFlightShop.IO;
using TheFlightShop.Weather;

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

            var connectionString = GetConnectionString();
            services.AddScoped<IProductDAL>(_ => new ProductDAL(connectionString));
            services.AddScoped<IOrderDAL>(_ => new OrderDAL(connectionString));

            var emailApiKey = Environment.GetEnvironmentVariable("EMAIL_API_KEY");
            var username = Environment.GetEnvironmentVariable("EMAIL_FROM_USERNAME");
            var from = Environment.GetEnvironmentVariable("EMAIL_FROM_NAME") ?? "The Flight Shop";
            var emailDomain = Environment.GetEnvironmentVariable("EMAIL_DOMAIN");
            var adminAddress = Environment.GetEnvironmentVariable("EMAIL_ADMIN_ADDRESS");
            services.AddScoped<IEmailClient>(_ => new MailgunEmailClient(emailApiKey, username, from, emailDomain, adminAddress));

            services.AddScoped<IHash>(_ => new Hash());

            var tokenIssuer = Environment.GetEnvironmentVariable("AUTH_ISSUER");
            var tokenAudience = Environment.GetEnvironmentVariable("AUTH_AUDIENCE");
            var signingKey = Environment.GetEnvironmentVariable("AUTH_KEY");
            services.AddScoped<IToken>(_ => new Token(tokenIssuer, tokenAudience, signingKey));

            var accessKeyId = Environment.GetEnvironmentVariable("S3_ACCESS_KEY_ID");
            var secretAccessKey = Environment.GetEnvironmentVariable("S3_SECRET_ACCESS_KEY");
            var productContentBucketName = Environment.GetEnvironmentVariable("S3_PRODUCT_CONTENT_BUCKET_NAME");
            var productContentRegion = Environment.GetEnvironmentVariable("S3_PRODUCT_CONTENT_REGION");
            services.AddScoped<IFileManager>(_ => new AwsS3FileManager(accessKeyId, secretAccessKey, productContentBucketName, productContentRegion));

            var apiKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY");
            var latitude = Environment.GetEnvironmentVariable("WEATHER_LATITUDE");
            var longitude = Environment.GetEnvironmentVariable("WEATHER_LONGITUDE");
            services.AddSingleton<IWeatherClient>(_ => new DarkSkyWeatherClient(apiKey, latitude, longitude));
        }

        private string GetConnectionString()
        {
            var connectionStringTemplate = Configuration.GetConnectionString("FlightShopData");
            var databaseUrl = Environment.GetEnvironmentVariable("CLEARDB_DATABASE_URL");

            var username = "FlightShopAdmin";
            var password = "fly2mySHOP!";
            var host = "localhost";
            var schema = "FlightShopData";

            if (databaseUrl != null && databaseUrl.Length > 0)
            {
                var urlParts = databaseUrl.Split('@');
                var credentials = urlParts[0].Split("mysql://")[1].Split(':');
                username = credentials[0];
                password = credentials[1];
                var urlPath = urlParts[1].Split('/');
                host = urlPath[0];
                schema = urlPath[1].Split('?')[0];
            }

            return string.Format(connectionStringTemplate, host, schema, username, password);
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
