using InterviewSiddhant_Gauchan.Handlers;
using InterviewSiddhant_Gauchan.Helpers;
using InterviewSiddhant_Gauchan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InterviewSiddhant_Gauchan
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
            services.Configure<Config>(Configuration.GetSection("TrueLayerApi"));
            services.AddAuthentication("testScheme").AddIdentityServerAuthentication(options =>
            {
                options.Authority = "https://auth.truelayer.com";
                options.RequireHttpsMetadata = true;
            }).AddCookie(options => 
                {
                    options.Cookie.HttpOnly = true;                    
                });

            services.AddSingleton<IStorage, Storage>();
            services.AddSingleton<IHttpHelpers, HttpHelpers>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<ITransactionService, TransactionService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ITransactionHandler, TransactionHandler>();
            services.AddTransient<ITransactionSummaryHandler, TransactionSummaryHandler>();
            services.AddTransient<ITokenHandler, TokenHandler>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {

                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("https://auth.truelayer.com"),
                           // TokenUrl = new Uri("https://auth.truelayer.com/connect/token"),
                            // RefreshUrl=new Uri("https://auth.truelayer.com/connect/token")

                        }
                    }

                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
                                               
                c.OperationFilter<AuthResponsesOperationFilter>();

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IOptions<Config> config)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();

            app.UseMvc();
            app.UseSwagger();


            app.UseSwaggerUI(c =>
            {
                c.OAuthClientId(config.Value.ClientId);
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.OAuthConfigObject = new OAuthConfigObject
                {
                    ClientId = config.Value.ClientId,
                    ClientSecret = config.Value.ClientSecret,

                    AdditionalQueryStringParams = new Dictionary<string, string>
                    {
                        { "enable_mock", "true" },
                        { "scope", "transactions accounts offline_access info" },
                        {"response_type","code"},
                        //{"redirect_uri",$"{config.Value.RedirectUri}"},
                    }
                };
                c.OAuth2RedirectUrl(config.Value.RedirectUri);
                c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
            });
            var cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
            };
            app.UseCookiePolicy(cookiePolicyOptions);
        }


    }
    public class AuthResponsesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AuthorizeAttribute>();

            if (authAttributes.Any())
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
        }
        
    }

}
