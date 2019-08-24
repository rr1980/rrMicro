using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
//using rrMicro.Core.HttpSender;

namespace rrMicro.Services.TestService
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
            services.AddControllers();
            services.AddHostedService<HttpSenderHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class HttpSenderHostedService : IHostedService
    {
        HttpClient client = new HttpClient();

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //while (!cancellationToken.IsCancellationRequested)
            //{
            //    await Task.Run(Execute, cancellationToken);
            //    var task = new Task(() => Execute(), TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness);
            //}
            await Connect(cancellationToken);
        }

        private async Task Connect(CancellationToken cancellationToken)
        {
            var requestMessage = new HttpRequestMessage();
            requestMessage.RequestUri = new Uri("http://localhost:5000/Intern/Register");
            requestMessage.Method = HttpMethod.Post;

            using (var responseMessage = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                var content = await responseMessage.Content.ReadAsStringAsync();

            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
