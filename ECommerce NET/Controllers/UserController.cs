using ECommerce_NET.Dto;
using ECommerce_NET.Interfaces;
using ECommerce_NET.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce_NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _userService;

        public UserController(IUser userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        [ProducesResponseType(204)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<UserLimitedDto>))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromForm] UserDto user)
        {
            var isUserAuthenticated = await _userService.AuthenticateUser(user.Username, user.Password);

            if (isUserAuthenticated.authenticated)
            {
                var userLogged = await _userService.GetUser(user.Username);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userLogged.User_Id.ToString()),
                    new Claim(ClaimTypes.Name, userLogged.Username),
                    new Claim(ClaimTypes.Role, userLogged.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                var loginResponse = new LoginResponseDto
                {
                    IsAuthenticated = isUserAuthenticated.authenticated, 
                    Message = isUserAuthenticated.result,
                    User = isUserAuthenticated.user
                };

                return Ok(loginResponse);
            }

            else if (!isUserAuthenticated.authenticated)
            {
                var loginResponseUnauthorized = new LoginResponseDto
                {
                    IsAuthenticated = isUserAuthenticated.authenticated,
                    Message = isUserAuthenticated.result,
                    User = isUserAuthenticated.user
                };

                return Unauthorized(loginResponseUnauthorized);
            }

            return NoContent();
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromForm] User user, [FromForm] IFormFile file)
        {
            var newUser = await _userService.RegisterUser(user, file);

            return Ok(new { Created = newUser });
        }

        [Authorize]
        [HttpPatch("{userId}/update")]
        [ProducesResponseType(401)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateUser(int userId, [FromForm] UserUpdateDto user, [FromForm] IFormFile? file)
        {
            // * Extract ID from claims for access control
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _ = int.TryParse(userIdClaim, out int id);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (id == userId)
            {
                var updatedProfile = await _userService.UpdateUser(userId, user, file);
                return NoContent();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
