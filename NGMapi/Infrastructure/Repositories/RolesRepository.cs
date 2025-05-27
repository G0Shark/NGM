using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Domain.Entities;
using NGMapi.Domain.Interfaces;
using NGMapi.Infrastructure.Persistence;
using NGMapi.Infrastructure.Services;

namespace NGMapi.Infrastructure.Repositories;

public class RolesRepository : IRolesRepository
{
    private readonly IRedisCacheService _redisCacheService;
    private readonly NGMDbContext _ngmDbContext;

    public RolesRepository(IRedisCacheService REDIS_CACHE_SERVICE, NGMDbContext NGM_DB_CONTEXT)
    {
        _redisCacheService = REDIS_CACHE_SERVICE;
        _ngmDbContext = NGM_DB_CONTEXT;
    }

    public int GetLevelOfRole(int ID)
    {
        return _ngmDbContext.roles.First(x => x.ID == ID).LEVEL;
    }
    public int UserMaxLevel(int USERID, int SERVERID)
    {
        var roles = GetRolesOfUser(USERID, SERVERID);
        int level = 100000;
        
        foreach (var role in roles)
        {
            if (role.LEVEL < level)
            {
                level = role.LEVEL;
            }
        }
        
        return level;
    }
    public bool IsUserHavePermission(int USERID, int SERVERID, string PERMISSION)
    {
        var roles = GetRolesOfUser(USERID, SERVERID);
        bool hasPermission = false;
        
        foreach (var role in roles)
        {
            if (role.PERMISSIONS.Contains(PERMISSION))
            {
                hasPermission = true;
            }
        }
        
        return hasPermission;
    }
    public Role[] GetRolesOfUser(int USERID, int SERVERID)
    {
        string[] rIds = _ngmDbContext.members.First(x => x.USERID == USERID && x.SERVERID == SERVERID).ROLEID.Split(' ');
        
        List<Role> roles = new List<Role>();
        foreach (var rId in rIds)
        {
            roles.Add(_ngmDbContext.roles.First(x => x.ID == int.Parse(rId)));
        }
        
        return roles.ToArray();
    }
    
    public Role GetRole(int ID)
    {
        if (_redisCacheService.Exists($"role_{ID}"))
            return _redisCacheService.Get<Role>($"role_{ID}");
        
        if (_ngmDbContext.roles.Any(x => x.ID == ID))
            return _ngmDbContext.roles.First(x => x.ID == ID);
        
        throw new NGMServiceExeption(new NotFoundObjectResult("Роли не найдено"));
    }
    
    public bool IsLevelExists(int SERVERID, int LEVEL)
    {
        if (_ngmDbContext.roles.Any(x => x.LEVEL == LEVEL && x.SERVERID == SERVERID))
            return true;
        
        return false;
    }
    
    public int GetServerIDByRoleID(int ID)
    {
        return _ngmDbContext.roles.First(x => x.ID == ID).SERVERID;
    }
    
    public void CreateRole(Role role)
    {
        _ngmDbContext.Add(role);
        _ngmDbContext.SaveChanges();
    }
    
    public void DeleteRole(int ID)
    {
        Role r = _ngmDbContext.roles.First(x => x.ID == ID);
        _ngmDbContext.roles.Remove(r);
        _ngmDbContext.SaveChanges();
        
        _redisCacheService.Remove($"role_{ID}");
    }

    public void DeleteAllRolesOfServer(int SERVERID)
    {
        Role[] rs = _ngmDbContext.roles.Where(x => x.SERVERID == SERVERID).ToArray();

        foreach (var r in rs)
        {
            _ngmDbContext.roles.Remove(r);
            _redisCacheService.Remove($"role_{r.ID}");
        }
        
        _ngmDbContext.SaveChanges();
    }
    
    public void ChangeName(int ID, string NAME)
    {
        Role r = _ngmDbContext.roles.First(x => x.ID == ID);
        r.NAME = NAME;
        _ngmDbContext.SaveChanges();
        _redisCacheService.Set($"role_{ID}", r);
    }
    
    public void ChangeLevel(int ID, int LEVEL)
    {
        Role r = _ngmDbContext.roles.First(x => x.ID == ID);
        r.LEVEL = LEVEL;
        _ngmDbContext.SaveChanges();
        _redisCacheService.Set($"role_{ID}", r);
    }
    
    public void ChangePermissions(int ID, string PERMISSIONS)
    {
        Role r = _ngmDbContext.roles.First(x => x.ID == ID);
        r.PERMISSIONS = PERMISSIONS;
        _ngmDbContext.SaveChanges();
        _redisCacheService.Set($"role_{ID}", r);
    }
}