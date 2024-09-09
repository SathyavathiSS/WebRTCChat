// Models/UpdateProfileDto.cs
using System.ComponentModel.DataAnnotations;

namespace UserService.Models
{
    public class UpdateProfileDto
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
    }
}
