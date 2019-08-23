using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using rrMicro.Common.Token;
using rrMicro.Gateway.Main.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace rrMicro.Gateway.Main
{
    public static class UriExtension
    {
        public static Uri AttachParameters(this Uri uri, NameValueCollection parameters)
        {
            var stringBuilder = new StringBuilder();
            string str = "?";
            for (int index = 0; index < parameters.Count; ++index)
            {
                stringBuilder.Append(str + parameters.AllKeys[index] + "=" + parameters[index]);
                str = "&";
            }
            return new Uri(uri + stringBuilder.ToString());
        }
    }

    public class Router
    {
        public List<Route> Routes { get; set; }
        public string AuthenticationPath { get; set; }


        public Router(string routeConfigFilePath)
        {
            dynamic router = JsonLoader.LoadFromFile<dynamic>(routeConfigFilePath);

            Routes = JsonLoader.Deserialize<List<Route>>(Convert.ToString(router.routes));
            AuthenticationPath = (JsonLoader.Deserialize<Destination>(Convert.ToString(router.authenticationService)) as Destination).Path;
        }

        public async Task<HttpResponseMessage> RouteRequest(HttpRequest request)
        {
            string path = request.Path.ToString();
            string basePath = '/' + path.Split('/')[1];

            Destination destination;
            try
            {
                destination = Routes.First(r => r.Endpoint.Equals(basePath)).Destination;
            }
            catch
            {
                return ConstructErrorMessage("The path could not be found.");
            }

            if (destination.RequiresAuthentication)
            {
                string token = request.Headers["token"];

                var username = TokenManager.ValidateToken(token);

                using (var client = new HttpClient())
                {
                    var uri = new Uri(AuthenticationPath)
                        .AttachParameters(new NameValueCollection
                            {
                            {"token", token},
                            {"username", username}
                        });

                    var authResponse = await client.GetAsync(uri);
                    if (!authResponse.IsSuccessStatusCode) return ConstructErrorMessage("Authentication failed.");
                }
            }

            return await destination.SendRequest(request);
        }

        private HttpResponseMessage ConstructErrorMessage(string error)
        {
            HttpResponseMessage errorMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent(error)
            };
            return errorMessage;
        }

    }
}