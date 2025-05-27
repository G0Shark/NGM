using NGMapi.Domain.Entities;

namespace NGMapi.Domain.Interfaces;

public interface IAuthRepository
{
    public void Register(User user);
    public User GetUser(string username);
    public User GetUserWithPassword(string username);
    public User GetUser(int id);
    public void DeleteUser(string username);
    public void UpdateUsername(string username, string newUsername);
    public void UpdatePassword(string username, string newPassword);
    public void UpdateDescription(string username, string newDescription);
    public void UpdateEmail(string username, string newEmail);
    public void UpdateDisplayName(string username, string newDisplayName);
}