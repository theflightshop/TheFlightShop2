using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using TheFlightShop.Auth;
using TheFlightShop.DAL;
using TheFlightShop.Email;
using TheFlightShop.IO;
using TheFlightShop.Logging;
using TheFlightShop.Payment;
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

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddHttpClient();

            var connectionString = GetConnectionString();
            var maintenanceSubdir = Environment.GetEnvironmentVariable("S3_MAINTENANCE_ITEMS_SUB_DIRECTORY");
            if (string.IsNullOrEmpty(maintenanceSubdir))
            {
                maintenanceSubdir = "maintenance";
            }

            services.AddScoped<IProductDAL>(provider => new ProductDAL(connectionString, maintenanceSubdir, provider.GetRequiredService<ILogger<ProductDAL>>()));
            services.AddScoped<IOrderDAL>(provider => new OrderDAL(connectionString, provider.GetRequiredService<ILogger<OrderDAL>>()));
            services.AddScoped(provider =>
            {
                var clientFactory = provider.GetRequiredService<IHttpClientFactory>();
                return new NmiPaymentGateway(clientFactory, Configuration["PaymentGateway:Url"], Configuration["PaymentGateway:ApiKey"], provider.GetRequiredService<ILogger<NmiPaymentGateway>>());
            });

            var emailApiKey = Environment.GetEnvironmentVariable("EMAIL_API_KEY");
            var username = Environment.GetEnvironmentVariable("EMAIL_FROM_USERNAME");
            var from = Environment.GetEnvironmentVariable("EMAIL_FROM_NAME") ?? "The Flight Shop";
            var emailDomain = Environment.GetEnvironmentVariable("EMAIL_DOMAIN");
            var adminAddress = Environment.GetEnvironmentVariable("EMAIL_ADMIN_ADDRESS");
            services.AddScoped<IEmailClient>(provider => new MailgunEmailClient(emailApiKey, username, from, emailDomain, adminAddress, provider.GetRequiredService<ILogger<MailgunEmailClient>>()));

            services.AddScoped<IHash>(_ => new Hash());

            var tokenIssuer = Environment.GetEnvironmentVariable("AUTH_ISSUER");
            var tokenAudience = Environment.GetEnvironmentVariable("AUTH_AUDIENCE");
            var signingKey = Environment.GetEnvironmentVariable("AUTH_KEY");
            services.AddScoped<IToken>(_ => new Token(tokenIssuer, tokenAudience, signingKey));

            var accessKeyId = Environment.GetEnvironmentVariable("S3_ACCESS_KEY_ID");
            var secretAccessKey = Environment.GetEnvironmentVariable("S3_SECRET_ACCESS_KEY");
            var productContentBucketName = Environment.GetEnvironmentVariable("S3_PRODUCT_CONTENT_BUCKET_NAME");
            var productContentRegion = Environment.GetEnvironmentVariable("S3_PRODUCT_CONTENT_REGION");
            services.AddScoped<IFileManager>(provider => new AwsS3FileManager(accessKeyId, secretAccessKey, productContentBucketName, productContentRegion, provider.GetRequiredService<ILogger<AwsS3FileManager>>()));

            var apiKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY");
            var latitude = Environment.GetEnvironmentVariable("WEATHER_LATITUDE");
            var longitude = Environment.GetEnvironmentVariable("WEATHER_LONGITUDE");
            services.AddSingleton<IWeatherClient>(provider => new DarkSkyWeatherClient(apiKey, latitude, longitude, provider.GetRequiredService<ILogger<AwsS3FileManager>>()));
        }

        private string GetConnectionString()
        {
            var connectionStringTemplate = Configuration.GetConnectionString("FlightShopData");
            var databaseUrl = Environment.GetEnvironmentVariable("CLEARDB_DATABASE_URL");

            var username = "FlightShopAdmin";
            var password = "fly2mySHOP!";
            var host = "localhost";
            var schema = "flightshop-copy";

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
            // this should be the first step in the pipeline so most errors are caught and reported 
            app.UseExceptionHandler(error => error.Run(async context =>
            {
                await Task.Run(() =>
                {
                    var currentException = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    if (currentException != null)
                    {
                        var errorId = new PseudoUniqueId().Next();
                        var logger = LogManager.GetCurrentClassLogger();
                        if (currentException.GetType() == typeof(FlightShopActionException))
                        {
                            logger.Error(currentException.InnerException, $"[errorId={errorId}] {currentException.Message}");
                        }
                        else
                        {
                            logger.Error(currentException);
                        }
                        context.Response.Redirect($"/Home/Error/{errorId}");
                    }
                });
            }));

            app.UseForwardedHeaders();
            app.Use(async (context, next) =>
            {
                if (context.Request.IsHttps || context.Request.Headers["X-Forwarded-Proto"] == Uri.UriSchemeHttps)
                {
                    await next();
                }
                else
                {
                    string queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty;
                    var https = "https://" + context.Request.Host + context.Request.Path + queryString;
                    context.Response.Redirect(https);
                }
            });

            if (env.IsDevelopment())
            {
                // for debugging, if you don't want to use prod error page and email of logs to test address and storage of logs in dev s3 bucket (as of jan 17 2021)
                //app.UseDeveloperExceptionPage();
                NLogBuilder.ConfigureNLog("nlog.Development.config");
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
