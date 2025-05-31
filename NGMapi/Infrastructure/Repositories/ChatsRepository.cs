using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild;
using NGMapi.Domain.Entities;
using NGMapi.Domain.Interfaces;
using NGMapi.Infrastructure.Persistence;
using NGMapi.Infrastructure.Services;

namespace NGMapi.Infrastructure.Repositories;

public class ChatsRepository : IChatsRepository
{
    private readonly IRedisCacheService _redisCacheService;
    private readonly NGMDbContext _ngmDbContext;

    public ChatsRepository(IRedisCacheService REDIS_CACHE_SERVICE, NGMDbContext NGM_DB_CONTEXT)
    {
        _redisCacheService = REDIS_CACHE_SERVICE;
        _ngmDbContext = NGM_DB_CONTEXT;
    }

    public bool IsChatExists(int ID)
    {
        if (_redisCacheService.Exists($"chat_{ID}"))
            return true;

        if (_ngmDbContext.chats.Any(x => x.ID == ID))
        {
            _redisCacheService.Set($"chat_{ID}", _ngmDbContext.chats.First(x => x.ID == ID));
            return true;
        }
        
        return false;
    }
    
    public void CreateChat(Chat chat)
    {
        _ngmDbContext.chats.Add(chat);
        _ngmDbContext.SaveChanges();
    }
    
    public void DeleteChat(int ID)
    {
        _ngmDbContext.chats.Remove(_ngmDbContext.chats.First(x => x.ID == ID));
        _ngmDbContext.SaveChanges();
        
        if (_redisCacheService.Exists($"chat_{ID}"))
            _redisCacheService.Remove($"chat_{ID}");
    }
    
    public void UpdateName(int ID, string NAME)
    {
        Chat chat = _ngmDbContext.chats.First(x => x.ID == ID);
        chat.NAME = NAME;
        _ngmDbContext.SaveChanges();
        
        _redisCacheService.Set($"chat_{ID}", chat);
    }
    
    public void UpdateDescription(int ID, string DESCRIPTION)
    {
        Chat chat = _ngmDbContext.chats.First(x => x.ID == ID);
        chat.DESCRIPTION = DESCRIPTION;
        _ngmDbContext.SaveChanges();
        
        _redisCacheService.Set($"chat_{ID}", chat);
    }
    
    public void UpdateRoles(int ID, string ROLEID)
    {
        Chat chat = _ngmDbContext.chats.First(x => x.ID == ID);
        chat.ROLEID = ROLEID;
        _ngmDbContext.SaveChanges();
        
        _redisCacheService.Set($"chat_{ID}", chat);
    }
    
    public Chat[] GetChats(int SERVERID)
    {
        return _ngmDbContext.chats.Where(x => x.SERVERID == SERVERID).ToArray();
    }
    
    public string GetRolesForChat(int ID)
    {
        if (_redisCacheService.Exists($"chat_{ID}"))
            return _redisCacheService.Get<Chat>($"chat_{ID}").ROLEID;

        Chat c = _ngmDbContext.chats.First(x => x.ID == ID);
        _redisCacheService.Set($"chat_{ID}", c);
        return c.ROLEID;
    }
    
    public Chat GetChat(int ID)
    {
        if (_redisCacheService.Exists($"chat_{ID}"))
            return _redisCacheService.Get<Chat>($"chat_{ID}");
        
        Chat c = _ngmDbContext.chats.First(x => x.ID == ID);
        _redisCacheService.Set($"chat_{ID}", c);
        return c;
    }
}