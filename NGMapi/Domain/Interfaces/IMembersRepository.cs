using NGM_Server_rebuild.Domain.Entities;

namespace NGMapi.Domain.Interfaces;

public interface IMembersRepository
{
    public void CreateMember(int SERVERID, int USERID, string DISPLAYNAME);
    public void SetTextMute(int userId, int serverID, bool value);
    public void SetVoiceMute(int userId, int serverID, bool value);
    public bool IsMember(int serverid, int userid);
    public bool IsTextMuted(int serverid, int userid);
    public bool IsVoiceMuted(int serverid, int userid);
    public void DeleteAllMembersOfServer(int SERVERID);
    public void Delete(int serverid, int userid);
    public void DeleteUser(int userid);
    public Member GetMember(int id);
    public Member[] GetMembersOfServer(int SERVERID);
    public Member[] GetMembersOfUser(int USERID);
}