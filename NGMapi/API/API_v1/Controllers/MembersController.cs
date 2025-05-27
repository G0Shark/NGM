using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Domain.Entities;
using NGMapi.Domain.Entities;
using NGMapi.Domain.Interfaces;
using NGMapi.Infrastructure.Repositories;

namespace NGMapi.API.API_v1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MembersController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly IServersRepository _serversRepository;
    private readonly IMembersRepository _membersRepository;
    private readonly IRolesRepository _rolesRepository;

    public MembersController(IAuthRepository AUTH_REPOSITORY, IServersRepository SERVERS_REPOSITORY,
        IMembersRepository MEMBERS_REPOSITORY, IRolesRepository ROLES_REPOSITORY)
    {
        _authRepository = AUTH_REPOSITORY;
        _serversRepository = SERVERS_REPOSITORY;
        _membersRepository = MEMBERS_REPOSITORY;
        _rolesRepository = ROLES_REPOSITORY;
    }
    
    [HttpPost("join")]
    public ActionResult JoinServer(int SERVERID, string JOINPASSWORD)
    {
        User user = _authRepository.GetUser(User.Identity.Name);

        if (!_serversRepository.IsServerExists(SERVERID))
        {
            return new NotFoundObjectResult("Сервера не существует");
        }

        if (_serversRepository.IsVisible(SERVERID))
        {
            _membersRepository.CreateMember(SERVERID, user.ID, user.DISPLAYNAME);
        }
        else
        {
            if (_serversRepository.TryJoinWithPassword(SERVERID, JOINPASSWORD))
            {
                _membersRepository.CreateMember(SERVERID, user.ID, user.DISPLAYNAME);
            }
            else
            {
                return new UnauthorizedObjectResult("Пароль неверный");
            }
        }
            
        return new OkObjectResult("Вы присоеденились.");
    }
    
    [HttpPost("try_join")]
    public ActionResult TryJoinServer(int SERVERID, string JOINPASSWORD)
    {
        User user = _authRepository.GetUser(User.Identity.Name);

        if (!_serversRepository.IsServerExists(SERVERID))
        {
            return new NotFoundObjectResult("Сервера не существует");
        }

        if (_serversRepository.IsVisible(SERVERID))
        {
            _membersRepository.CreateMember(SERVERID, user.ID, user.DISPLAYNAME);
            return new OkObjectResult("Вы присоеденились.");
        }
        if (_serversRepository.TryJoinWithPassword(SERVERID, JOINPASSWORD))
        {
            _membersRepository.CreateMember(SERVERID, user.ID, user.DISPLAYNAME);
            return new OkObjectResult("Вы присоеденились.");
        }
        return new UnauthorizedObjectResult("Пароль неверный");
    }
    
    [HttpPost("leave")]
    public ActionResult LeaveServer(int SERVERID)
    {
        User user = _authRepository.GetUser(User.Identity.Name);

        if (!_membersRepository.IsMember(SERVERID, user.ID))
        {
            return new BadRequestObjectResult("Вы не участник сервера.");
        }
            
        _membersRepository.Delete(SERVERID, user.ID);
        return new OkObjectResult("Вы покинули сервер.");
    }
    
    [HttpPut("textmute")]
    public ActionResult TextMute(int USERID, int SERVERID, bool VALUE)
    {
        User user = _authRepository.GetUser(User.Identity.Name);

        if (!_membersRepository.IsMember(SERVERID, USERID))
        {
            return new BadRequestObjectResult("Данный пользователь не участник сервера.");
        }
            
        if (!_serversRepository.IsOwner(SERVERID, user.ID) && 
            !_rolesRepository.IsUserHavePermission(USERID, SERVERID, "MUTE_TEXT"))
        {
            return new UnauthorizedObjectResult("Нет прав.");
        }
            
        _membersRepository.SetTextMute(USERID, SERVERID, VALUE);
            
        return new OkObjectResult("Пользователь замьючен.");
    }
    
    [HttpPut("voicemute")]
    public ActionResult VoiceMute(int USERID, int SERVERID, bool VALUE)
    {
        User user = _authRepository.GetUser(User.Identity.Name);

        if (!_membersRepository.IsMember(SERVERID, USERID))
        {
            return new BadRequestObjectResult("Данный пользователь не участник сервера.");
        }
            
        if (!_serversRepository.IsOwner(SERVERID, user.ID) && 
            !_rolesRepository.IsUserHavePermission(USERID, SERVERID, "MUTE_VOICE"))
        {
            return new UnauthorizedObjectResult("Нет прав.");
        }
            
        _membersRepository.SetVoiceMute(USERID, SERVERID, VALUE);
            
        return new OkObjectResult("Пользователь замьючен.");
    }
    
    //TODO: Возможность кикать людей, и банить
    [HttpGet("get_byid")]
    public ActionResult<Member> GetMember(int ID)
    {
        Member member = _membersRepository.GetMember(ID);
        return new OkObjectResult(member);
    }
    
    [HttpGet("get_byserverid")]
    public ActionResult<List<Member>> GetMembers(int SERVERID)
    {
        return new OkObjectResult(_membersRepository.GetMembersOfServer(SERVERID));
    }
    
    [HttpGet("get_byuserid")]
    public ActionResult<List<Member>> GetMembersOfUser(int USERID)
    {
        return new OkObjectResult(_membersRepository.GetMembersOfUser(USERID));
    }
}