using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Models;
using WebApplication1.Models.Requests;
using WebApplication1.Repository;

namespace WebApplication1.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserRepository _repository;
        private readonly IConfiguration _configuration;
        public AuthController(IUserRepository repository, IConfiguration configuration)
        {
            _configuration = configuration;
            _repository = repository;
        }
        [HttpGet("/login")]
        public IActionResult Login()
        {
            return View("~/Views/Auth/Login.cshtml");
        }

        [HttpGet("/register")]
        public IActionResult Register()
        {
            return View("~/Views/Auth/Register.cshtml");
        }
        [HttpPost("login")]
        public async Task<ActionResult> Authenticate([FromBody] AuthenticateRequestDto request)
        {
            if (String.IsNullOrWhiteSpace(request.email) || String.IsNullOrWhiteSpace(request.password)) return Unauthorized("Invalid Email or Password");
            var user = await _repository.GetUserByEmailAsync(request.email);
            if (user == null) return Unauthorized("Invalid Email or Password");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.password, user.HashedPassword);
            if (!isPasswordValid) return Unauthorized("Invalid Email or Password");
            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (String.IsNullOrWhiteSpace(request.name)
                || String.IsNullOrWhiteSpace(request.password)
                || String.IsNullOrWhiteSpace(request.email)) 
                return BadRequest("Field(s) must not be blank");
            string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            string passwordHashedWithSalt = BCrypt.Net.BCrypt.HashPassword(request.password, salt);
            var user = new Models.User
            {
                Name = request.name,
                HashedPassword = passwordHashedWithSalt,
                Email = request.email,
                IsActive = true,
            };
            await _repository.CreateUserAsync(user);
            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }


        private string GenerateJwtToken(User user)
        {
            if (user.Id == 0) 
            {
                throw new InvalidOperationException("Cannot generate a token for a user without a valid ID.");
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim("isActive", user.IsActive? "1" : "0"),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
