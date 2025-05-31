using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Domain.Entities;
using NGMapi.Domain.Interfaces;
using NGMapi.Infrastructure.Persistence;
using NGMapi.Infrastructure.Services;

namespace NGMapi.Infrastructure.Repositories;

public class ServersRepository : IServersRepository
{
    private readonly IRedisCacheService _redisCacheService;
    private readonly NGMDbContext _ngmDbContext;

    public ServersRepository(IRedisCacheService REDIS_CACHE_SERVICE, NGMDbContext NGM_DB_CONTEXT)
    {
        _redisCacheService = REDIS_CACHE_SERVICE;
        _ngmDbContext = NGM_DB_CONTEXT;
    }

    public bool TryJoinWithPassword(int ID, string JOINPASSWORD)
    {
        if (_ngmDbContext.servers.Any(x => x.ID == ID && x.JOINPASSWORD == JOINPASSWORD))
            return true;
        return false;
    }
    public bool IsVisible(int ID)
    {
        if (_redisCacheService.Exists($"server_{ID}"))
            return _redisCacheService.Get<Server>($"server_{ID}").VISIBLE;

        Server s = _ngmDbContext.servers.First(x => x.ID == ID);
        _redisCacheService.Set($"server_{ID}", s);
        return s.VISIBLE;
    }
    public bool IsServerExists(int ID)
    {
        if (_redisCacheService.Exists($"server_{ID}"))
            return true;

        if (_ngmDbContext.servers.Any(x => x.ID == ID))
        {
            Server s = _ngmDbContext.servers.First(x => x.ID == ID);
            _redisCacheService.Set($"server_{ID}", s);
            return true;
        }
        
        return false;
    }
    
    public bool IsServerExists(string NAME)
    {
        if (_ngmDbContext.servers.Any(x => x.NAME == NAME))
        {
            Server s = _ngmDbContext.servers.First(x => x.NAME == NAME);
            _redisCacheService.Set($"server_{s.ID}", s);
            return true;
        }
        return false;
    }
    
    public Server GetServer(string NAME)
    {
        if (_ngmDbContext.servers.Any(x => x.NAME == NAME))
        {
            Server s = _ngmDbContext.servers.First(x => x.NAME == NAME);
            _redisCacheService.Set($"server_{s.ID}", s);
            return s;
        }
        
        throw new NGMServiceExeption(new NotFoundObjectResult("Сервера не найдено"));
    }
    
    public Server GetServer(int SERVERID)
    {
        if (_redisCacheService.Exists($"server_{SERVERID}"))
            return _redisCacheService.Get<Server>($"server_{SERVERID}");

        if (_ngmDbContext.servers.Any(x => x.ID == SERVERID))
        {
            Server s = _ngmDbContext.servers.First(x => x.ID == SERVERID);
            _redisCacheService.Set($"server_{SERVERID}", s);
            return s;
        }

        throw new NGMServiceExeption(new NotFoundObjectResult("Сервера не найдено"));
    }
    
    public bool IsOwner(int ID, int OWNERID)
    {
        if (_redisCacheService.Exists($"server_{ID}"))
            return _redisCacheService.Get<Server>($"server_{ID}").OWNERID == OWNERID;

        if (_ngmDbContext.servers.Any(x => x.ID == ID))
        {
            Server s = _ngmDbContext.servers.First(x => x.ID == ID);
            _redisCacheService.Set($"server_{ID}", s);
            return s.OWNERID == OWNERID;
        }
        
        throw new NGMServiceExeption(new NotFoundObjectResult("Сервера не найдено"));
    }
    
    public void CreateServer(Server server)
    {
        if (string.IsNullOrEmpty(server.DESCRIPTION))
        {
            server.DESCRIPTION = "Нет описания, ну думаю что это крутой сервер!";
        }
        
        _ngmDbContext.servers.Add(server);
        _ngmDbContext.SaveChanges();
    }

    public void ChangeName(int ID, string NAME)
    {
        Server s = _ngmDbContext.servers.First(x => x.ID == ID);
        s.NAME = NAME;
        _ngmDbContext.SaveChanges();
        
        _redisCacheService.Set($"server_{ID}", s);
    }
    
    public void ChangeDescription(int ID, string DESCRIPTION)
    {
        Server s = _ngmDbContext.servers.First(x => x.ID == ID);
        s.DESCRIPTION = DESCRIPTION;
        _ngmDbContext.SaveChanges();
        
        _redisCacheService.Set($"server_{ID}", s);
    }
    
    public void ChangeOwner(int ID, int OWNERID)
    {
        Server s = _ngmDbContext.servers.First(x => x.ID == ID);
        s.OWNERID = OWNERID;
        _ngmDbContext.SaveChanges();
        
        _redisCacheService.Set($"server_{ID}", s);
    }
    
    public void ChangeJoinPassword(int ID, string JOINPASSWORD)
    {
        Server s = _ngmDbContext.servers.First(x => x.ID == ID);
        s.JOINPASSWORD = JOINPASSWORD;
        _ngmDbContext.SaveChanges();
        
        _redisCacheService.Set($"server_{ID}", s);
    }
    
    public void ChangeVisible(int ID, bool VISIBLE)
    {
        Server s = _ngmDbContext.servers.First(x => x.ID == ID);
        s.VISIBLE = VISIBLE;
        _ngmDbContext.SaveChanges();
        
        _redisCacheService.Set($"server_{ID}", s);
    }
    
    public void DeleteServer(int ID)
    {
        _ngmDbContext.servers.Remove(_ngmDbContext.servers.First(x => x.ID == ID));
        _ngmDbContext.SaveChanges();
        
        _redisCacheService.Remove($"server_{ID}");
    }

    public Server[] GetServers()
    {
        return _ngmDbContext.servers.Where(x => x.VISIBLE == true).ToArray();
    }
}