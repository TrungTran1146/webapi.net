using WebAPI.Model;
using WebAPI.Model.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APIDemo.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly DataContext _context;

        public UserController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new Response
            {
                Success = true,
                Message = "Register success",

            });
        }
        [HttpPost("login")]
        public IActionResult Validate(LoginModel model)
        {
            // var user = _context.Users.SingleOrDefault(
            //  p =>p.UserName == model.Username && model.Password ==p.Password);
            var existingUser = _context.Users.SingleOrDefault(
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
//           // new Claim("FullName", existingUser.FullName),


            //roles
            new Claim("TokenId", Guid.NewGuid().ToString())
                    // new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            //////////////
            //cấp token
            return Ok(new Response
            {
                Success = true,
                Message = "Authenticte success",
                // Data = GenerateJwtToken(user)
                Data = tokenString
            });

        }
//        //[Authorize(Policy = "admin")]
//        //[Authorize]
//        //[HttpPost("logout")]
//        //public async Task<IActionResult> Logout()
//        //{
//        //    var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
//        //    if (token != null)
//        //    {
//        //        // invalidate token
//        //        await _tokenService.InvalidateTokenAsync(token);

//        //        return Ok(new { message = "Logout successful" });
//        //    }

//        //    return BadRequest(new { message = "Invalid request" });
//        //}
//        //private string GenerateJwtToken(User user)
//        //{
//        //    var tokenHandler = new JwtSecurityTokenHandler();
//        //    var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);
//        //    var tokenDescriptor = new SecurityTokenDescriptor
//        //    {
//        //        Subject = new ClaimsIdentity(new Claim[]
//        //        {
//        //    new Claim("ProductId", user.ProductId.ToString()),
//        //    new Claim(ClaimTypes.Name, user.UserName),
//        //    new Claim(ClaimTypes.Email,user.Email),
//        //    new Claim("FullName", user.FullName),


//        //    //roles
//        //    new Claim("TokenId", Guid.NewGuid().ToString())
//        //            // new Claim(ClaimTypes.Role, user.Role)
//        //        }),
//        //        Expires = DateTime.UtcNow.AddDays(1),
//        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//        //    };

//        //    var token = tokenHandler.CreateToken(tokenDescriptor);
//        //    var tokenString = tokenHandler.WriteToken(token);

//        //    return tokenString;
//        //}

   }
}
