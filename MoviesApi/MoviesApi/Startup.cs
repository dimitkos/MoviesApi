using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MoviesApi.Filters;
using MoviesApi.Services;
using System.IO;

namespace MoviesApi
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
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddAutoMapper(typeof(Startup));

            //it is when i use azure storage
            //services.AddTransient<IFileStorageService, AzureStorageService>();

            //if i use wwwroot to save the pictures
            services.AddTransient<IFileStorageService, InAppStorageService>();
            services.AddHttpContextAccessor();

            services.AddControllers(options => 
            {
                options.Filters.Add(typeof(ExceptionFilter)); //apply global a filter
            }).AddXmlDataContractSerializerFormatters();
            //services.AddResponseCaching();
            services.AddTransient<LoggingActionFilter>();
            //services.AddTransient<IHostedService, WriteToFileHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            //app.Use(async (context, next) =>
            //{
            //    using (var swapStream = new MemoryStream())
            //    {
            //        var originalResponseBody = context.Response.Body;
            //        context.Response.Body = swapStream;

            //        await next.Invoke();

            //        swapStream.Seek(0, SeekOrigin.Begin);
            //        string responseBody = new StreamReader(swapStream).ReadToEnd();
            //        swapStream.Seek(0, SeekOrigin.Begin);

            //        await swapStream.CopyToAsync(originalResponseBody);
            //        context.Response.Body = originalResponseBody;

            //        logger.LogInformation(responseBody);
            //    }
            //});


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            //if i use wwwroot to store pictures
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseResponseCaching();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
