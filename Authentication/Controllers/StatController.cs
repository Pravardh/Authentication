using Authentication.Services;
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
        public async Task<IActionResult> GetStat([FromBody] GetStatModel getStatsInfo)
        {
            return Ok(await PlayerStatService.GetStatValue(getStatsInfo.Username, getStatsInfo.Key));
        }
    }
}
