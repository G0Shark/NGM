using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Domain.Entities;
using NGMapi.Domain.Interfaces;
using NGMapi.Infrastructure.Persistence;
using NGMapi.Infrastructure.Services;

namespace NGMapi.Infrastructure.Repositories;


public class MembersRepository : IMembersRepository
{
    private readonly IRedisCacheService _redisCacheService;
    private readonly NGMDbContext _ngmDbContext;

    public MembersRepository(IRedisCacheService REDIS_CACHE_SERVICE, NGMDbContext NGM_DB_CONTEXT)
    {
        _redisCacheService = REDIS_CACHE_SERVICE;
        _ngmDbContext = NGM_DB_CONTEXT;
    }

    public void CreateMember(Member member)
    {
        member.ROLEID = "-1";
        
        _ngmDbContext.Add(member);
        _ngmDbContext.SaveChanges();
    }
    
    public void SetTextMute(int USERID, int SERVERID, bool VALUE)
    {
        _ngmDbContext.members.First(x => x.USERID == USERID || x.SERVERID == SERVERID).TEXTMUTE = VALUE;
        _ngmDbContext.SaveChanges();
    }
    
    public void SetVoiceMute(int USERID, int SERVERID, bool VALUE)
    {
        _ngmDbContext.members.First(x => x.USERID == USERID && x.SERVERID == SERVERID).VOICEMUTE = VALUE;
        _ngmDbContext.SaveChanges();
    }

    public bool IsMember(int SERVERID, int USERID)
    {
        if (_ngmDbContext.members.Any(x => x.USERID == USERID && x.SERVERID == SERVERID))
            return true;
        
        return false;
    }
    
    public bool IsTextMuted(int SERVERID, int USERID)
    {
        return _ngmDbContext.members.First(x => x.USERID == USERID && x.SERVERID == SERVERID).TEXTMUTE;
    }
    
    public bool IsVoiceMuted(int SERVERID, int USERID)
    {
        return _ngmDbContext.members.First(x => x.USERID == USERID && x.SERVERID == SERVERID).VOICEMUTE;
    }
    public void DeleteAllMembersOfServer(int SERVERID)
    {
        Member[] ms = _ngmDbContext.members.Where(x => x.SERVERID == SERVERID).ToArray();

        foreach (var m in ms)
        {
            _ngmDbContext.Remove(m);
        }
        
        _ngmDbContext.SaveChanges();
    }
    
    public void Delete(int SERVERID, int USERID)
    {
        _ngmDbContext.Remove(_ngmDbContext.members.First(x => x.USERID == USERID &&  x.SERVERID == SERVERID));
    }
    
    public void DeleteUser(int USERID)
    {
        Member[] ms = _ngmDbContext.members.Where(x => x.USERID == USERID).ToArray();

        foreach (var m in ms)
        {
            _ngmDbContext.Remove(m);
        }
        
        _ngmDbContext.SaveChanges();
    }

    public Member GetMember(int ID)
    {
        if (_redisCacheService.Exists($"member_{ID}"))
            return _redisCacheService.Get<Member>($"member_{ID}");
        
        if  (_ngmDbContext.members.Any(x => x.ID == ID))
            return _ngmDbContext.members.First(x => x.ID == ID);
        
        throw new NGMServiceExeption(new NotFoundObjectResult("Member не найден"));
    }

    public Member[] GetMembersOfServer(int SERVERID)
    {
        return _ngmDbContext.members.Where(x => x.SERVERID == SERVERID).ToArray();
    }
    
    public Member[] GetMembersOfUser(int USERID)
    {
        return _ngmDbContext.members.Where(x => x.USERID == USERID).ToArray();
    }
}