namespace NGM_Server_rebuild.Models;

public class User
{
    public string id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; }
}