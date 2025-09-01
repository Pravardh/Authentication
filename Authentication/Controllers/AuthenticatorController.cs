using Authentication.Services;
using Authentication.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.Controllers
{
    #region Models
    public class RegisterModel()
    {
        public required string Username { get; set; }
        public required string Name { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }   

    }

    public class LoginModel()
    {
        public  required string Username { get; set; }
        public  required string Password { get; set; }    
    }
    public class ChangePasswordModel()
    {
        public  required string Username { get; set; }
        public  required string Password { get; set; }    
        public  required string NewPassword { get; set; }    
    }
    public class GetUserModel()
    {
        public required string Username;
    }
    public class AuthenticatedResponse
    {
        public string? Token { get; set; }
    }
    #endregion


    [ApiController]
    [Route("[controller]")]
    public class AuthenticatorController : ControllerBase
    {
        private readonly ILogger<AuthenticatorController> _logger;

        public AuthenticatorController(ILogger<AuthenticatorController> logger)
        {
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel userToRegister)
        {
            if (await UserService.Register(userToRegister.Username, userToRegister.Name, userToRegister.Email, userToRegister.Password))
            {
                _logger.Log(LogLevel.Information, "Registered Successfully");
                return Ok("User created successfully");
            }
            else
            {
                _logger.Log(LogLevel.Warning, "User already exists");
                return Conflict("User already exists");
            }

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel userToLogin)
        {
            if (string.IsNullOrEmpty(userToLogin.Username) || string.IsNullOrEmpty(userToLogin.Password))
            {
                return BadRequest("Username and password are required");
            }

            bool loginResult = await UserService.Login(userToLogin.Username, userToLogin.Password);

            if (loginResult)
            {
                _logger.Log(LogLevel.Information, $"User {userToLogin.Username} logged in successfully");
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345OLEOLEOLEolesadasdasdadjk2333@"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(
                    issuer: "http://localhost:5097",
                    audience: "http://localhost:5097",
                    claims: new List<Claim>(),
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signinCredentials
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

                return Ok(tokenString);
            }
            else
            {
                _logger.Log(LogLevel.Warning, $"Login failed for user {userToLogin.Username}");
                return Unauthorized("Cannot login. Username or password is incorrect");
            }
        }

        
        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel userPasswordData)
        {
            if (string.IsNullOrEmpty(userPasswordData.Username) || string.IsNullOrEmpty(userPasswordData.Password) || string.IsNullOrEmpty(userPasswordData.NewPassword))
            {
                return BadRequest("Username, password and new password is required");
            }

            string currentPassword = userPasswordData.Password;
            string newPassword = userPasswordData.NewPassword;

            string currentPasswordHash = Hasher.GetHashString(currentPassword);
            string newPasswordHash = Hasher.GetHashString(newPassword);

            if (currentPassword == newPassword)
            {
                return BadRequest("Current password cannot be the same as new password");
            }
            

            if (await UserService.ChangePassword(userPasswordData.Username, userPasswordData.Password, userPasswordData.NewPassword))
            {
                return Ok("Password changed successfully");
            }
            return BadRequest("Password is wrong or new password is not strong enough. Failed to change password");
        }



        [HttpPost("DeleteUser")]
        [Authorize]
        public async Task<IActionResult> DeleteUser([FromBody] LoginModel userToLogin)
        {
            if (string.IsNullOrEmpty(userToLogin.Username) || string.IsNullOrEmpty(userToLogin.Password))
            {
                return BadRequest("Username and password is required to delete");
            }

            bool deleteResult = await UserService.DeleteUser(userToLogin.Username, userToLogin.Password);

            if (deleteResult)
            {
                _logger.Log(LogLevel.Information, $"User {userToLogin.Username} deleted successfully");
                return Ok("Successfully deleted");
            }
            else
            {
                _logger.Log(LogLevel.Warning, $"Login failed for user {userToLogin.Username}");
                return Unauthorized("Cannot login. Username or password is incorrect");
            }
        }

        public async Task<IActionResult> PrintAllUsers()
        {
            await UserService.PrintAllUsers();

            return Ok();
        }
    }
}
