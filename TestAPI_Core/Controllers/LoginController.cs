using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TestAPI_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginController> _logger;

        public LoginController(IConfiguration configuration, ILogger<LoginController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        [HttpGet("login")]
        public IActionResult Login([FromQuery] string empusername, string emppassword)
        {
            string connectionString = _configuration.GetConnectionString("Test_db");
            using (SqlConnection sc = new SqlConnection(connectionString))
            {
                string query = "select EmpPwd from student where Email=@Email";
                SqlCommand cmd = new SqlCommand(query, sc);
                cmd.Parameters.AddWithValue("@Email", empusername);
                sc.Open();
                object result = cmd.ExecuteScalar();
                string password = result != null ? result.ToString() : null;

                if (password == emppassword)
                {
                    _logger.LogInformation("User {username} logged in successfully.", empusername);
                    var token = GenerateJwtToken(empusername);
                    return Ok(new { Token = token });
                }
                else
                {
                    _logger.LogWarning("Invalid login attempt for user: {username}", empusername);
                    return Unauthorized("Invalid credentials");
                }
            }
        }

        private string GenerateJwtToken(string username)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Admin") 
                }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

}
