using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Domain.Entities;
using NGMapi.Domain.Interfaces;
using NGMapi.Infrastructure.Persistence;
using NGMapi.Infrastructure.Services;

namespace NGMapi.Infrastructure.Repositories;

public class MessagingRepository : IMessagingRepository
{
    private readonly IRedisCacheService _redisCacheService;
    private readonly NGMDbContext _ngmDbContext;

    public MessagingRepository(IRedisCacheService REDIS_CACHE_SERVICE, NGMDbContext NGM_DB_CONTEXT)
    {
        _redisCacheService = REDIS_CACHE_SERVICE;
        _ngmDbContext = NGM_DB_CONTEXT;
    }

    public bool IsUserAreSender(int ID, int SENDERID)
    {
        if (_ngmDbContext.messages.Any(x => x.ID == ID && x.SENDERID == SENDERID))
            return true;
        return false;
    }
    
    public bool IsMessageExists(int ID)
    {
        if (_ngmDbContext.messages.Any(x => x.ID == ID))
            return true;
        return false;
    }
    
    public Message SendMessage(Message message)
    {
        _ngmDbContext.messages.Add(message);
        _ngmDbContext.SaveChanges();
        
        return message;
    }
    
    public void EditMessage(int ID, string NEWCONTENT)
    {
        if (!IsMessageExists(ID))
            throw new NGMServiceExeption(new NotFoundObjectResult("Сообщения не существует"));
        
        Message m = _ngmDbContext.messages.First(x => x.ID == ID);
        m.MESSAGE = NEWCONTENT;
        _ngmDbContext.SaveChanges();
        
        _redisCacheService.Set($"message_{ID}", m);
    }
    
    public void DeleteMessage(int ID)
    {
        if (!IsMessageExists(ID))
        {
            throw new NGMServiceExeption(new NotFoundObjectResult("Сообщения не существует"));
        }
        
        Message m =  _ngmDbContext.messages.First(x => x.ID == ID);
        _ngmDbContext.messages.Remove(m);
        _ngmDbContext.SaveChanges();
        
        _redisCacheService.Remove($"message_{ID}");
    }

    public Message[] GetMessages(int CHATID, bool DMCHAT)
    {
        return _ngmDbContext.messages.Where(x => x.CHATID == CHATID && x.DMCHAT == DMCHAT).ToArray();
    }
    
    public Message[] GetMessagesFromDM(int CHATID1, int CHATID2)
    {
        return _ngmDbContext.messages.Where(x => x.CHATID == CHATID1 || x.CHATID == CHATID2 && x.DMCHAT == true ).ToArray();
    }
    
    public Message GetMessage(int ID)
    {
        if (_redisCacheService.Exists($"message_{ID}"))
            return _redisCacheService.Get<Message>($"message_{ID}");
        
        if (_ngmDbContext.messages.Any(x => x.ID == ID))
            return _ngmDbContext.messages.First(x => x.ID == ID);
        
        throw new NGMServiceExeption(new NotFoundObjectResult("Сообщения не существует"));
    }
}