using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NavigationModule.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

[ApiController]
[Route("api/user")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public UserController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isRegistered = await _userService.Register(model);

            if (isRegistered)
            {
                return Ok(new { message = "User registered successfully." });
            }

            return BadRequest(new { message = "Failed to register user." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to register user.", error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool loggedIn = await _userService.Login(model);

            if (loggedIn)
            {
                var user = await _userService.GetUserByUsername(model.Username);

                if (user != null)
                {
                    //add claims of user 
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id)
                    };

                    var userRoles = await _userService.GetUserRoles(user);
                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                    var expiry = DateTime.Now.AddHours(5);

                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: expiry,
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = expiry
                    });
                }

                return NotFound(new { message = "User not found!" });
            }

            return Unauthorized(new { message = "Invalid credentials." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to log in user.", error = ex.Message });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            // Call the service method to log out the user
            await _userService.LogoutAsync();
            return Ok("User logged out successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to log out user.", error = ex.Message });
        }
    }
}