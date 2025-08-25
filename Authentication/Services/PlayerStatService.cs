using Authentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Services
{
    public class PlayerStatService
    {
        public static async Task<bool> SaveStat(string username, string key, string value)
        {
            if (await UserService.DoesUserExit(username))
            {
                using (var db = new UserContext())
                {
                    var user = await db.UserSet.Include(u => u.PlayerStats)
                        .FirstOrDefaultAsync(u => u.Username == username);

                    if (user != null)
                    {
                        var playerStat = await db.PlayerStats.FirstOrDefaultAsync(u => u.KeyName == key);

                        if (playerStat == null)
                        {
                            //Create new player stat
                            playerStat = new PlayerStat
                            {
                                Username = username,
                                KeyName = key,
                                Value = value
                            }; 
                            user.PlayerStats.Add(playerStat);
                        }
                        else
                        {
                            playerStat.Value = value;
                        }

                        await db.SaveChangesAsync();

                        return true;
                    }
                }
            }
            return false;
        }

        public static async Task<string> GetStatValue(string username, string keyName)
        {
            using (var db = new UserContext())
            {
                var user = await db.UserSet.Include(u => u.PlayerStats)
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user != null)
                {
                    var playerStat = await db.PlayerStats.FirstOrDefaultAsync(u => u.KeyName == keyName);

                    if (playerStat != null)
                    {
                        return playerStat.Value;
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }

        public static async Task<bool> DeleteStat(string username, string keyName)
        {
            using (var db = new UserContext())
            {
                var user = await db.UserSet.Include(u => u.PlayerStats)
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user != null)
                {
                    var playerStat = await db.PlayerStats.FirstOrDefaultAsync(u => u.KeyName == keyName);

                    if(playerStat == null)
                    {
                        Console.WriteLine("Player stat does not exist");
                        return false;
                    }
                    
                    db.PlayerStats.Remove(playerStat);

                    await db.SaveChangesAsync();

                }
                else
                {
                    return false;
                }
            }
            return true;

        }

        public static async Task PrintAllStats()
        {
            using (var db = new UserContext())
            {

                var playerStats = await db.PlayerStats.ToListAsync();

                if (playerStats.Count == 0)
                {
                    Console.WriteLine("No stats found.");
                    return;
                }

                foreach (var u in playerStats)
                {
                    Console.WriteLine($"Username: {u.Username}, Key: {u.KeyName}, Value: {u.Value}");
                }
            }
        }




    }
}
