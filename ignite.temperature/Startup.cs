using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Dapr;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Temperature.Infrastructure;

namespace Temperature
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

            services.AddDaprClient();

            services.AddSingleton(new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            });

            services.AddOptions();
            services.AddCors();
            services.AddMvc();

            services.AddSingleton<TemperatureHub>();

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, JsonSerializerOptions serializerOptions, TemperatureHub temperatureHub)
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

            app.UseRouting();

            app.UseCors(builder =>
               builder
               .AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod());

            // app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<TemperatureHub>("/temperatureHub");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapSubscribeHandler();

                endpoints.MapPost("readings-input", Readings).WithTopic("readings");
            });

            async Task Readings(HttpContext context)
            {
                var tempReading = await JsonSerializer.DeserializeAsync<Reading>(context.Request.Body, serializerOptions);
                var reading = new
                {
                    Date = DateTime.Now,
                    Temperature = tempReading.Temperature
                };

                await temperatureHub.SendMessage(JsonSerializer.Serialize(reading));

                context.Response.ContentType = "application/json";
                await JsonSerializer.SerializeAsync(context.Response.Body, string.Empty, serializerOptions);
            }
        }
    }

    class Reading
    {
        public double Temperature { get; set; }
    }
}