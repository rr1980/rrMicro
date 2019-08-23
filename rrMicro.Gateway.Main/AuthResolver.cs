using Microsoft.AspNetCore.Http;
using rrMicro.Common.Models;
using rrMicro.Database.Repositories;
using rrMicro.Gateway.Main.Utils;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace rrMicro.Gateway.Main
{
    public static class AuthResolver
    {
        public static async Task<string> Login(HttpRequest request)
        {
            string requestContent;
            using (Stream receiveStream = request.Body)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    requestContent = await readStream.ReadToEndAsync();
                }
            }

            var user = JsonLoader.Deserialize<User>(requestContent);

            User u = new UserRepository().GetUser(user.Username);
            if (u == null)
            {
                return null;
            }

            bool credentials = u.Password.Equals(user.Password);
            if (!credentials)
            {
                return null;
            }

            return TokenManager.GenerateToken(user.Username);
        }

        public static bool Validate(HttpRequest request)
        {
            string token = request.Headers["token"];
            var username = TokenManager.ValidateToken(token);

            return new UserRepository().GetUser(username) != null;
        }
    }
}