using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using DocumentTracking.API.DBContext;
using DocumentTracking.API.Services;
using DocumentTracking.API.Classes;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace DocumentTracking.API
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
            services.AddHttpCacheHeaders((expirationModelOptions) =>
            {
                expirationModelOptions.MaxAge = 60;
                expirationModelOptions.CacheLocation = Marvin.Cache.Headers.CacheLocation.Private;
            },
            (validationModelOption) =>
            {
                validationModelOption.MustRevalidate = true;
            });
            services.AddResponseCaching();

            //use th addController to configure what you want to configure
            services.AddControllers(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;
                setupAction.CacheProfiles.Add("240SecondsCacheProfile",
                    new CacheProfile()
                    {
                        Duration = 240
                    });
            })
                .AddNewtonsoftJson(setupAction =>
                {
                    setupAction.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlDataContractSerializerFormatters()
                
                .ConfigureApiBehaviorOptions(setupAction =>
                {
                    setupAction.InvalidModelStateResponseFactory = context =>
                    {
                        var problemDetailsFactory = context.HttpContext.RequestServices
                        .GetRequiredService<ProblemDetailsFactory>();
                        var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                                context.HttpContext,
                                context.ModelState
                            );

                        //add additional infor not added by default
                        problemDetails.Detail = "See the error fields for details.";
                        problemDetails.Instance = context.HttpContext.Request.Path;

                        //fill out which status code to use
                        var actionExecutionContext =
                        context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                        //if there are modelstate error & all argument were correct
                        //found/parsed we 're dealing with validation errors
                        if ((context.ModelState.ErrorCount > 0) &&
                        (actionExecutionContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count))
                        {
                            problemDetails.Type = "http://documenttrack.waec.org.ng/modelvalidationproblem";
                            problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                            problemDetails.Title = "One or more validation error occured";

                            return new UnprocessableEntityObjectResult(problemDetails)
                            {
                                ContentTypes = { "application/problem+json" }
                            };
                        }

                        //if one of the arguement was not correctly found/couldn't be found
                        //we are dealing with null/unparsable input
                        problemDetails.Status = StatusCodes.Status400BadRequest;
                        problemDetails.Title = "One or more error on input occured.";
                        return new BadRequestObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    };
                });

            services.Configure<MvcOptions>(config =>
            {
                var newtonsoftJsonOutputFormatter = config.OutputFormatters
                .OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                if(newtonsoftJsonOutputFormatter != null)
                {
                    newtonsoftJsonOutputFormatter.
                    SupportedMediaTypes.Add("application/vnd.waec.hateoas+json");
                }
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //define the connection
            var connection = Configuration.GetConnectionString("DocTrackingConnection");
            services.AddDbContext<WDocumentTrackingContext>(options =>
            {
                options.UseSqlServer(connection);
            });

            //create a connection sting vaue
            var connectionString = new ConnectionString(connection);

            //register the PropertyMappingService
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();

            //register the PropertyMappingService
            services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();

            //the commented code uses dapper
            //services.AddScoped<IUsersRepository, UsersRepository>(provider => new UsersRepository(connectionString));
            services.AddScoped<IUsersRepository, cUsersRepository>();
            services.AddScoped<IAuditTrailsRepository, cAuditTrailsRepository>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                    });
                });
            }

            //activate caching
            //app.UseResponseCaching();

            //this must be added after the cache store
            app.UseHttpCacheHeaders();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
