using JwtAuthLearn.Entities;
using JwtAuthLearn.Models;
using JwtAuthLearn.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthLearn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register(UserDto request)
        {
            var user = await authService.RegisterAsync(request);

            if(user is null)
                return BadRequest("User already exists");

            return Ok(new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
        {
            var result = await authService.LoginAsync(request);

            if (result is null)
                return BadRequest("Invalid username or password.");

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result =
                await authService.RefreshTokensAsync(request);

            if (result is null ||
                result.AccessToken is null ||
                result.RefreshToken is null)
            {
                return Unauthorized("Invalid refresh token.");
            }

            return Ok(result);
        }

        // Any authenticated user
        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            return Ok("You are authenticated!");
        }

        // Only Admin
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are an admin!");
        }

        // Only Manager
        [Authorize(Policy = "ManagerOnly")]
        [HttpGet("manager-only")]
        public IActionResult ManagerOnlyEndpoint()
        {
            return Ok("You are a manager!");
        }

        // TeamLead OR Developer
        [Authorize(Policy = "TeamLeadOrDeveloper")]
        [HttpGet("teamlead-or-developer")]
        public IActionResult TeamLeadOrDeveloperEndpoint()
        {
            return Ok("You are a TeamLead or Developer!");
        }
    }
}