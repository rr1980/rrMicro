using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace rrMicro.Core.HttpSender
{
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
                var content = await responseMessage.Content.ReadAsByteArrayAsync();

            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
