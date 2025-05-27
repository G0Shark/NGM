using NGM_Server_rebuild.Domain.Entities;

namespace NGMapi.Domain.Interfaces;

public interface IRolesRepository
{
    public void CreateRole(Role role);
    public void ChangeName(int id, string name);
    public void ChangePermissions(int id, string permissions);
    public void ChangeLevel(int id, int level);
    public void DeleteRole(int id);
    public void DeleteAllRolesOfServer(int serverID);
    public int GetServerIDByRoleID(int ID);
    public bool IsLevelExists(int serverid, int level);
    public Role GetRole(int id);
    public Role[] GetRolesOfUser(int userID, int serverID);
    public int GetLevelOfRole(int ID);
    public bool IsUserHavePermission(int userID, int serverID, string permission);
    public int UserMaxLevel(int userID, int serverID);
}