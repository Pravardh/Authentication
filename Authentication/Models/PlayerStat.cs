namespace Authentication.Models
{
    public class PlayerStat
    {
        public int PlayerStatID { get; set; }
        
        public required string KeyName { get; set; }
        public required string Value { get; set; }

        public required string Username { get; set; }    
        public User User { get; set; }


    }
}
