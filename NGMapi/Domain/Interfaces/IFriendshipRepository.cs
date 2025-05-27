using NGM_Server_rebuild.Domain.Entities;

namespace NGMapi.Domain.Interfaces;

public interface IFriendshipRepository
{
    public Friendship GetFriendship(int id);
    public void DeleteUser(int userId);
    public void CreateFriendship(Friendship friendship);
    public void SetFriendshipState(int senderId, int accepterId, int state);
    public bool IsBlocked(int senderId, int accepterId);
    public bool IsFriend(int senderId, int accepterId);
    public bool IsFriendRequest(int senderId, int accepterId);
    public bool IsFriendshipExists(int senderId, int accepterId);
}