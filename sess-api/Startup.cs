﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Authentication;
using sess_api.Tools;
using Microsoft.OpenApi.Models;
using NJsonSchema;
using NSwag.AspNetCore;

namespace sess_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var VarM = VarManager.Instance;
            
            VarM.Vars.Add("RedisConfig", configuration.GetSection("redis"));
            VarM.Vars.Add("Parameters", configuration.GetSection("parameters"));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            services.AddApiVersioning(o => o.ApiVersionReader = new HeaderApiVersionReader("api-version"));
            
            // configure basic authentication 
            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, Security.BasicAuthenticationHandler>("BasicAuthentication", null);


            // Register the Swagger services
            services.AddSwaggerDocument();


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
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            
            app.UseAuthentication();

            app.UseOpenApi();

            // Register the Swagger generator and the Swagger UI middlewares
            //app.UseSwagger();
            //app.UseSwaggerUi3();
            
            app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseMvc();
        }
    }
}