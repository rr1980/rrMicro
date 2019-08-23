using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace rrMicro.Gateway.Main
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            Router router = new Router("routes.json");
            app.Run(async (context) =>
            {
                string path = context.Request.Path.ToString();
                string basePath = '/' + path.Split('/')[1];
                string data;

                if (basePath == "/Login")
                {
                    data = await AuthResolver.Login(context.Request);
                }
                else
                {
                    var content = await router.RouteRequest(context.Request);
                    data = await content.Content.ReadAsStringAsync();
                }

                await context.Response.WriteAsync(data);
            });
        }
    }
}
