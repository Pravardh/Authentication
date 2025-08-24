using Authentication.Utils;
using Microsoft.EntityFrameworkCore;
using Authentication.Models;

namespace Authentication.Services
{
    public static class UserService
    {
        public static async Task<bool> Register(string username, string name, string email, string passwd)
        {

            using (var db = new UserContext())
            {
                var user = await db.UserSet.FirstOrDefaultAsync(u =>
                     u.Username.ToLower() == username.ToLower());

                if (user == null)
                {
                    user = new User()
                    {
                        Username = username,
                        Name = name,
                        EmailID = email,
                        Password = Hasher.GetHashString(passwd)
                    };

                    Console.WriteLine($"Username: {user.Username}, Email: {user.EmailID}, Password Hash: {user.Password}");
                    
                    db.Add(user);
                    await db.SaveChangesAsync();

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static async Task<bool> DeleteUser(string username, string password)
        {
            using (var db = new UserContext())
            {
                var user = await db.UserSet.FirstOrDefaultAsync(u =>
                    u.Username.ToLower() == username.ToLower());

                if (user != null)
                {
                    db.Remove(user);
                    db.SaveChanges();
                
                    return true;
                }
            }
            
            return false;
        }

        public static async Task<bool> DoesUserExit(string username)
        {
            using (var db = new UserContext())
            {
                var user = await db.UserSet.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

                return user != null;
            }
        }

        public static async Task<bool> Login(string username, string password)
        {
            using (var db = new UserContext())
            {

                var user = await db.UserSet.FirstOrDefaultAsync(u =>
                    u.Username.ToLower() == username.ToLower());

                if (user == null)
                {
                    Console.WriteLine("User does not exist");
                    return false;
                }
                else
                {
                    string storedHash = user.Password;
                    string hash = Hasher.GetHashString(password);

                    Console.WriteLine($"User Password: {storedHash}, Calculated Hash: {hash}");

                    if (storedHash.ToLower().Equals(hash.ToLower()))
                    {
                        Console.WriteLine("User logged in");
                        return true;
                    }

                    Console.WriteLine("User cannot be logged in");

                    return false;

                }
            }
        }

        public static async Task PrintAllUsers()
        {
            using (var db = new UserContext())
            {
                var users = await db.UserSet.ToListAsync();

                if (users.Count == 0)
                {
                    Console.WriteLine("No users found.");
                    return;
                }

                Console.WriteLine("=== Users in Database ===");
                foreach (var u in users)
                {
                    Console.WriteLine($"Username: {u.Username}, Email: {u.EmailID}, Name: {u.Name}");
                }
            }
        }


    }
}
