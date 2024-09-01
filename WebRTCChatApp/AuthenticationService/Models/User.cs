using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public required byte[] PasswordHash { get; set; } = Array.Empty<byte>();

    public byte[]  PasswordSalt { get; set; } = Array.Empty<byte>();

    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ProfilePicture { get; set; } = string.Empty;  // URL or path to profile picture
}

