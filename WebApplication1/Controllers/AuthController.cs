using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthRequest request)
        {
            // Hardcoded user for demo
            if (request.Username == "user" && request.Password == "password")
            {
                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("ChangeThis_VeryLong_Default_Key_For_Development"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    claims: new[] { new Claim(ClaimTypes.Name, request.Username) },
                    expires: DateTime.UtcNow.AddHours(2),
                    signingCredentials: creds
                );
                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new AuthResponse { Token = jwt });
            }
            return Unauthorized();
        }
    }
}
