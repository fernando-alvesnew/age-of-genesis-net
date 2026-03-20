namespace RecruiterApi.Domain.Users;

public class User
{
    public long Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string UserType { get; set; } = "player";
    public bool IsBanned { get; set; }
    public string? LastIp { get; set; }
}
