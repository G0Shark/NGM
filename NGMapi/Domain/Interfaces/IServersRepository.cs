using NGM_Server_rebuild.Domain.Entities;

namespace NGMapi.Domain.Interfaces;

public interface IServersRepository
{
    public void CreateServer(Server server);
    public Server GetServer(int ID);
    public Server GetServer(string name);
    public Server[] GetServers();
    public void ChangeName(int id, string NAME);
    public void ChangeDescription(int id, string DESCRIPTION);
    public void ChangeOwner(int id, int OWNERid);
    public void ChangeJoinPassword(int id, string joinpassword);
    public void ChangeVisible(int id, bool visible);
    public void DeleteServer(int ID);
    public bool IsOwner(int id, int ownerid);
    public bool IsServerExists(string name);
    public bool IsServerExists(int id);
    public bool IsVisible(int id);
    public bool TryJoinWithPassword(int id, string joinpassword);
}