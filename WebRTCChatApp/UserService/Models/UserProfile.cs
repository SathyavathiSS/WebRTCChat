namespace UserService.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Bio { get; set; }

        public UserProfile(string username, string email, string fullName, string bio)
        {
            Username = username;
            Email = email;
            FullName = fullName;
            Bio = bio;
        }
    }
}
