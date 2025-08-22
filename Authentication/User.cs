namespace Authentication
{
    public class User
    {
        public User() { }

        public User(string username, string emailID, string name, string password)
        {
            Username = username;
            EmailID = emailID;
            Name = name;
            Password = password;
        }

        public string Username { get; set; }
        public string EmailID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
