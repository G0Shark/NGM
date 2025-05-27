using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Domain.Entities;
using NGMapi.Domain.Interfaces;
using NGMapi.Infrastructure.Persistence;
using NGMapi.Infrastructure.Services;

namespace NGMapi.Infrastructure.Repositories;

public class FriendshipRepository : IFriendshipRepository
{
    private readonly IRedisCacheService _redisCacheService;
    private readonly NGMDbContext _ngmDbContext;

    public FriendshipRepository(IRedisCacheService REDIS_CACHE_SERVICE, NGMDbContext NGM_DB_CONTEXT)
    {
        _redisCacheService = REDIS_CACHE_SERVICE;
        _ngmDbContext = NGM_DB_CONTEXT;
    }

    public bool IsFriendshipExists(int SENDERID, int ACCEPTERID)
    {
        if (_ngmDbContext.friendships.Any(x => x.SENDERID == SENDERID && x.ACCEPTERID == ACCEPTERID))
            return true;
        
        return false;
    }
    
    public bool IsFriendRequest(int SENDERID, int ACCEPTERID)
    {
        if (_ngmDbContext.friendships.Any(x => x.SENDERID == SENDERID && x.ACCEPTERID == ACCEPTERID && x.STATE == 2))
            return true;
        
        return false;
    }
    
    public bool IsFriend(int SENDERID, int ACCEPTERID)
    {
        if (_ngmDbContext.friendships.Any(x => x.SENDERID == SENDERID && x.ACCEPTERID == ACCEPTERID && x.STATE == 3))
            return true;
        
        return false;
    }
    
    public bool IsBlocked(int SENDERID, int ACCEPTERID)
    {
        if (_ngmDbContext.friendships.Any(x => x.SENDERID == SENDERID && x.ACCEPTERID == ACCEPTERID && x.STATE == 4))
            return true;
        
        return false;
    }
    
    public void SetFriendshipState(int SENDERID, int ACCEPTERID, int STATE)
    {
        Friendship f = _ngmDbContext.friendships.First(x => x.SENDERID == SENDERID && x.ACCEPTERID == ACCEPTERID);
        f.STATE = STATE;
        _ngmDbContext.SaveChanges();
    }

    public void CreateFriendship(Friendship friendship)
    {
        _ngmDbContext.friendships.Add(friendship);
        _ngmDbContext.SaveChanges();
    }
    
    public void DeleteUser(int USERID)
    {
        Friendship[] fs = _ngmDbContext.friendships.Where(x => x.SENDERID == USERID || x.ACCEPTERID == USERID).ToArray();
        foreach (Friendship f in fs)
        {
            _ngmDbContext.friendships.Remove(f);
        }
        _ngmDbContext.SaveChanges();
    }

    public Friendship GetFriendship(int ID)
    {
        return _ngmDbContext.friendships.First(x => x.ID == ID);
    }
}