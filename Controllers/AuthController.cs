using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using COMP584StudyAbroadServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace COMP584StudyAbroadServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        // DTOs with validation
        public record RegisterDto(
            [Required, EmailAddress] string Email,
            [Required, MinLength(6)] string Password
        );

        public record LoginDto(
            [Required, EmailAddress] string Email,
            [Required] string Password
        );

        // POST: /api/Auth/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // default new users to "User" role
            await _userManager.AddToRoleAsync(user, "User");
            return Ok();
        }

        // POST: /api/Auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !(await _userManager.CheckPasswordAsync(user, dto.Password)))
                return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);

            var jwtSettings = _config.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecurityKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(ClaimTypes.Name, user.UserName!)
            };

            // add role claims
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = tokenString });
        }

        // GET: /api/Auth/me
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                Email = user.Email,
                Roles = roles
            });
        }
    }
}
