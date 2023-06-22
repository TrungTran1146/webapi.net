using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.Model;
using WebAPI.Model.Auth;

namespace WebAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly DataContext _dbContext;

        public AuthController(DataContext context, IConfiguration configuration)
        {
            _dbContext = context;
            _configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return Ok(new Response
            {
                Success = true,
                Message = "Register success",

            });
        }
        [HttpPost("login")]
        public IActionResult Validate(LoginModel model)
        {

            var existingUser = _dbContext.Users.SingleOrDefault(
                p => p.UserName == model.Username);
            if (!BCrypt.Net.BCrypt.Verify(model.Password, existingUser.Password))
            {
                return BadRequest("Invalid username or password");
            }
            if (existingUser == null)
            {
                return Ok(new Response
                {

                    Success = false,
                    Message = "Invalid username/password"
                });
            }
            /////////////
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim("Id", existingUser.Id.ToString()),
            new Claim(ClaimTypes.Name, existingUser.UserName),
            new Claim(ClaimTypes.Role, existingUser.IsAdmin ? "admin" : "user"),
            new Claim(ClaimTypes.Email,existingUser.Email),



            //roles
            new Claim("TokenId", Guid.NewGuid().ToString())
                    // new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            //cấp token
            return Ok(new Response
            {
                Success = true,
                Message = "Authenticte success",
                // Data = GenerateJwtToken(user)
                Data = new
                {
                    id = existingUser.Id,
                    name = existingUser.UserName,
                    role = existingUser.IsAdmin ? "admin" : "user",
                    token = tokenString
                }

            });

        }


    }
}
