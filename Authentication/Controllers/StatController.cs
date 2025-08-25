using Authentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Controllers
{
    #region Models
    public class SaveStatModel()
    {
        public required string Username { get; set; }
        public required string Key { get; set; }
        public required string Value { get; set; }
    }

    public class GetStatModel()
    {
        public required string Username { get; set; }
        public required string Key { get; set; }
    }
    #endregion

    [ApiController]
    [Route("[controller]")]
    public class StatController : ControllerBase
    {
        [HttpPost("SaveStat")]
        [Authorize]
        public async Task<IActionResult> SaveStat([FromBody] SaveStatModel saveStatsInfo)
        {
            if (await PlayerStatService.SaveStat(saveStatsInfo.Username, saveStatsInfo.Key, saveStatsInfo.Value))
            {
                Console.WriteLine($"{saveStatsInfo.Key} : {saveStatsInfo.Value}");
                return Ok("Stat has been successfully created!");
            }

            return BadRequest("Player stat could not be created successfully.");

        }

        [HttpPost("GetStat")]
        [Authorize]
        public async Task<IActionResult> GetStat([FromBody] GetStatModel getStatsInfo)
        {
            Console.WriteLine("GetStat endpoint hit - user is authenticated");
            Console.WriteLine($"User identity: {User.Identity.Name}");
            Console.WriteLine($"User claims count: {User.Claims.Count()}");

            return Ok(await PlayerStatService.GetStatValue(getStatsInfo.Username, getStatsInfo.Key));
        }
    }
}
