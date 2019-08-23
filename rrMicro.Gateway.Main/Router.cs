using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using rrMicro.Gateway.Main.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace rrMicro.Gateway.Main
{
    public class Router
    {
        public List<Route> Routes { get; set; }


        public Router(string routeConfigFilePath)
        {
            dynamic router = JsonLoader.LoadFromFile<dynamic>(routeConfigFilePath);

            Routes = JsonLoader.Deserialize<List<Route>>(Convert.ToString(router.routes));
        }

        public async Task RouteRequest(HttpContext context)
        {
            string path = context.Request.Path.ToString();
            string basePath = '/' + path.Split('/')[1];

            Destination destination;
            try
            {
                destination = Routes.First(r => r.Endpoint.Equals(basePath)).Destination;
            }
            catch
            {
                var data = await (ConstructErrorMessage("The path could not be found.")).Content.ReadAsStringAsync();
                await context.Response.WriteAsync(data);
                return;
            }

            if (destination.RequiresAuthentication)
            {
                if (!AuthResolver.Validate(context.Request))
                {
                    var data = await (ConstructErrorMessage("Authentication failed.")).Content.ReadAsStringAsync();
                    await context.Response.WriteAsync(data);
                    return;
                }
            }

            await destination.SendRequest(context);
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