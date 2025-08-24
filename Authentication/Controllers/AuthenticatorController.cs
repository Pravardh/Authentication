using Authentication.Services;
using Microsoft.AspNetCore.Mvc;

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
    public class GetUserModel()
    {
        public required string Username;
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
                return Ok("Successfully logged in");
            }
            else
            {
                _logger.Log(LogLevel.Warning, $"Login failed for user {userToLogin.Username}");
                return Unauthorized("Cannot login. Username or password is incorrect");
            }
        }

        [HttpPost("Delete")]
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
