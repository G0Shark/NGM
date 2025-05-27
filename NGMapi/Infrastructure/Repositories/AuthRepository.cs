using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Domain.Entities;
using NGMapi.Application.Interfaces;
using NGMapi.Domain.Entities;
using NGMapi.Domain.Interfaces;
using NGMapi.Infrastructure.Persistence;
using NGMapi.Infrastructure.Services;

namespace NGMapi.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly IRedisCacheService _redisCacheService;
    private readonly NGMDbContext _ngmDbContext;
    private readonly IPasswordHasher _passwordHasher;
    
    public AuthRepository(IRedisCacheService REDIS_CACHE_SERVICE, NGMDbContext NGM_DB_CONTEXT, IPasswordHasher PASSWORD_HASHER)
    {
        _redisCacheService = REDIS_CACHE_SERVICE;
        _ngmDbContext = NGM_DB_CONTEXT;
        _passwordHasher = PASSWORD_HASHER;
    }

    private bool IsUserExists(string USERNAME)
    {
        if (_redisCacheService.Exists($"user_{USERNAME}"))
            return true;
        
        if (_ngmDbContext.users.Any(x => x.USERNAME == USERNAME))
            return true;
        return false;
    }
    
    public void Register(User user)
    {
        if (IsUserExists(user.USERNAME))
        {
            throw new NGMServiceExeption(new UnauthorizedObjectResult("USERNAME занят"));
        }

        _ngmDbContext.users.Add(user);
        _ngmDbContext.SaveChanges();
        _redisCacheService.Set($"user_{user.USERNAME}", user);
    }

    public User GetUser(string USERNAME)
    {
        if (_redisCacheService.Exists($"user_{USERNAME}"))
            return _redisCacheService.Get<User>($"user_{USERNAME}");
        
        if (_ngmDbContext.users.Any(x => x.USERNAME == USERNAME))
            return _ngmDbContext.users.First(x => x.USERNAME == USERNAME);
        
        throw new NGMServiceExeption(new NotFoundObjectResult("Пользователь не найден"));
    }
    public User GetUserWithPassword(string USERNAME)
    {
        if (_redisCacheService.Exists($"user_{USERNAME}"))
            return _redisCacheService.Get<User>($"user_{USERNAME}");
        
        if (_ngmDbContext.users.Any(x => x.USERNAME == USERNAME))
            return _ngmDbContext.users.First(x => x.USERNAME == USERNAME);
        
        throw new NGMServiceExeption(new NotFoundObjectResult("Пользователь не найден"));
    }
    public User GetUser(int ID)
    {
        if (_ngmDbContext.users.Any(x => x.ID == ID))
            return _ngmDbContext.users.First(x => x.ID == ID);
        
        throw new NGMServiceExeption(new NotFoundObjectResult("Пользователь не найден"));
    }
    public void DeleteUser(string USERNAME)
    {
        if (_redisCacheService.Exists($"user_{USERNAME}"))
            _redisCacheService.Remove($"user_{USERNAME}");
        
        _ngmDbContext.users.Remove(_ngmDbContext.users.First(x => x.USERNAME == USERNAME));
        _ngmDbContext.SaveChanges();
    }
    public void UpdateUsername(string USERNAME, string NEWUSERNAME)
    {
        if (IsUserExists(NEWUSERNAME))
        {
            throw new NGMServiceExeption(new BadRequestObjectResult("username занят"));
        }
        
        User u = _ngmDbContext.users.First(x => x.USERNAME == USERNAME);
        u.USERNAME = NEWUSERNAME;
        _ngmDbContext.SaveChanges();
        
        _redisCacheService.Set($"user_{USERNAME}", u);
    }
    
    public void UpdatePassword(string USERNAME, string PASSWORD)
    {
        User u = _ngmDbContext.users.First(x => x.USERNAME == USERNAME);
        u.PASSWORD = _passwordHasher.HashPassword(PASSWORD);
        _ngmDbContext.SaveChanges();
    }

    public void UpdateEmail(string USERNAME, string EMAIL)
    {
        User u = _ngmDbContext.users.First(x => x.EMAIL == EMAIL);
        u.EMAIL = EMAIL;
        _ngmDbContext.SaveChanges();
    }
    
    public void UpdateDisplayName(string USERNAME, string DISPLAYNAME)
    {
        User u = _ngmDbContext.users.First(x => x.USERNAME == USERNAME);
        u.DISPLAYNAME = DISPLAYNAME;
        _ngmDbContext.SaveChanges();
        
        _redisCacheService.Set($"user_{USERNAME}", u);
    }
    
    public void UpdateDescription(string USERNAME, string DESCRIPTION)
    {
        User u = _ngmDbContext.users.First(x => x.USERNAME == USERNAME);
        u.DESCRIPTION = DESCRIPTION;
        _ngmDbContext.SaveChanges();
        
        _redisCacheService.Set($"user_{USERNAME}", u);
    }
}