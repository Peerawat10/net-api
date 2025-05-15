using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;



namespace MainProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        // จำลองฐานข้อมูลไว้ในหน่วยความจำ
        private static readonly List<User> Users = [];

        // GET: /api/user
        [HttpGet]
        [Authorize]
        public IActionResult GetUsers()
        {
            return Ok(Users);
        }

        // POST: /api/user/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] User newUser)
        {
            if (Users.Any(u => u.Username == newUser.Username))
                return Conflict(new { message = "Username already exists" });

            Users.Add(newUser);
            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User login)
        {
            var user = Users.FirstOrDefault(u =>
                u.Username == login.Username && u.Password == login.Password);

            if (user == null)
                return Unauthorized(new { message = "Invalid credentials" });

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this_is_my_super_secret_key_for_jwt_token_123!@#"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                // claims: new[] { new Claim(ClaimTypes.Name, user.Username) },
                claims: [new Claim(ClaimTypes.Name, user.Username)],
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

    }
}
