using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoviesApi.Filters;
using MoviesApi.Helpers;
using MoviesApi.Services;
using System;
using System.IO;
using System.Reflection;
using System.Text;

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
            services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>(tags: new[] { "ready" }); //only checks for readiness

            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
            sqlserver => sqlserver.UseNetTopologySuite()));

            services.AddCors();
            services.AddDataProtection();

            services.AddAutoMapper(typeof(Startup));

            services.AddTransient<HashService>();

            //it is when i use azure storage
            //services.AddTransient<IFileStorageService, AzureStorageService>();

            //if i use wwwroot to save the pictures
            services.AddTransient<IFileStorageService, InAppStorageService>();
            services.AddHttpContextAccessor();

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["jwt:key"])),
                        ClockSkew = TimeSpan.Zero
                    });

            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ExceptionFilter)); //apply global a filter
            }).AddNewtonsoftJson()
              .AddXmlDataContractSerializerFormatters();
            //services.AddResponseCaching();
            services.AddTransient<LoggingActionFilter>();
            //services.AddTransient<IHostedService, WriteToFileHostedService>();
            services.AddTransient<IHostedService, MovieInTheaterService>();

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "MoviesAPI",
                    Description = "This is a Web API for Movies operations",
                    TermsOfService = new Uri("https://udemy.com/user/felipegaviln/"),
                    License = new OpenApiLicense()
                    {
                        Name = "MIT"
                    },
                    Contact = new OpenApiContact()
                    {
                        Name = "Felipe Gavilán",
                        Email = "felipe_gavilan887@hotmail.com",
                        Url = new Uri("https://gavilan.blog/")
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);
            });
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

            app.UseSwagger();

            app.UseSwaggerUI(config =>
            {
                config.SwaggerEndpoint("/swagger/v1/swagger.json", "MoviesAPI");
            });

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

            //this origin allows to use my api
            app.UseCors(builder =>
             builder.WithOrigins("http://apirequest.io")
                    .WithMethods("GET", "POST")
                    .AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    ResponseWriter = HealthCheckResponseWriter.WriteResponseReadiness,
                    Predicate = (check) => check.Tags.Contains("ready")
                });

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                {
                    ResponseWriter = HealthCheckResponseWriter.WriteResponseLiveness,
                    Predicate = (check) => !check.Tags.Contains("ready")
                });
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
