using NGMapi.Domain.Entities;

namespace NGMapi.Domain.Interfaces;

public interface IChatsRepository
{
    public Chat GetChat(int id);
    public Chat[] GetChats(int serverId);
    public string GetRolesForChat(int id);
    public void CreateChat(Chat chat);
    public void DeleteChat(int id);
    public bool IsChatExists(int id);
    public void UpdateName(int id, string name);
    public void UpdateDescription(int id, string description);
    public void UpdateRoles(int id, string roleid);
}