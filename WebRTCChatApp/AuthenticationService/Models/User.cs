using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public required byte[] PasswordHash { get; set; } = Array.Empty<byte>();

    public byte[]  PasswordSalt { get; set; } = Array.Empty<byte>();

    // Make these properties nullable
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfilePicture { get; set; }
}

