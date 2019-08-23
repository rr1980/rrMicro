using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using rrMicro.Common.Models;
using rrMicro.Common.Token;
using rrMicro.Database.Repositories;

namespace rrMicro.Services.AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        [Route("Login")]
        public ActionResult<string> Login(User user)
        {
            User u = new UserRepository().GetUser(user.Username);
            if (u == null)
            {
                return NotFound();
            }

            bool credentials = u.Password.Equals(user.Password);
            if (!credentials)
            {
                return Forbid();
            }

            return TokenManager.GenerateToken(user.Username);
        }

        [HttpGet]
        [Route("Validate")]
        public ActionResult Validate(string token, string username)
        {
            bool exists = new UserRepository().GetUser(username) != null;
            if (!exists)
            {
                return NotFound();
            }
            string tokenUsername = TokenManager.ValidateToken(token);
            if (username.Equals(tokenUsername))
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
