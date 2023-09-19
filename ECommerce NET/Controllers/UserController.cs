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
        [ProducesResponseType(200)]
        public async Task<IActionResult> Login([FromForm] UserDto user)
        {
            bool userExists = await _userService.UsernameExists(user.Username);

            if(!userExists)
            {
                return BadRequest();
            }

            bool isUserAuthenticated = await _userService.AuthenticateUser(user.Username, user.Password);

            if (isUserAuthenticated)
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

                return Ok();
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
            var idString = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            _ = int.TryParse(idString, out int id);

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

        [Authorize]
        [HttpGet("claims")]
        //[HttpPatch("update")]
        public async Task<IActionResult> Claims()
        {
            //var userClaims = HttpContext.User.Claims.Select(claim => new
            //{
            //    claim.Type,
            //    claim.Value
            //});

            //return Ok(userClaims);

            var id = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            return Ok(id);
        }
    }
}
