using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Domain.Entities;
using NGM_Server_rebuild.Services;
using NGMapi.Domain.Entities;
using NGMapi.Infrastructure.Repositories;

namespace NGMapi.API.API_v1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MembersController : ControllerBase
{
    [HttpPost("join")]
    public ActionResult JoinServer(int SERVERID, string JOINPASSWORD)
    {
        User user = AuthRepository.GetUser(User.Identity.Name);

        if (!ServersRepository.IsServerExists(SERVERID))
        {
            return new NotFoundObjectResult("Сервера не существует");
        }

        if (ServersRepository.IsVisible(SERVERID))
        {
            MembersRepository.Create(SERVERID, user.ID, user.DISPLAYNAME);
        }
        else
        {
            if (ServersRepository.TryJoinWithPassword(SERVERID, JOINPASSWORD))
            {
                MembersRepository.Create(SERVERID, user.ID, user.DISPLAYNAME);
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
        User user = AuthRepository.GetUser(User.Identity.Name);

        if (!ServersRepository.IsServerExists(SERVERID))
        {
            return new NotFoundObjectResult("Сервера не существует");
        }

        if (ServersRepository.IsVisible(SERVERID))
        {
            MembersRepository.Create(SERVERID, user.ID, user.DISPLAYNAME);
            return new OkObjectResult("Вы присоеденились.");
        }
        if (ServersRepository.TryJoinWithPassword(SERVERID, JOINPASSWORD))
        {
            MembersRepository.Create(SERVERID, user.ID, user.DISPLAYNAME);
            return new OkObjectResult("Вы присоеденились.");
        }
        return new UnauthorizedObjectResult("Пароль неверный");
    }
    
    [HttpPost("leave")]
    public ActionResult LeaveServer(int SERVERID)
    {
        User user = AuthRepository.GetUser(User.Identity.Name);

        if (!MembersRepository.IsMember(SERVERID, user.ID))
        {
            return new BadRequestObjectResult("Вы не участник сервера.");
        }
            
        MembersRepository.Delete(SERVERID, user.ID);
        return new OkObjectResult("Вы покинули сервер.");
    }
    
    [HttpPut("textmute")]
    public ActionResult TextMute(int USERID, int SERVERID, bool VALUE)
    {
        User user = AuthRepository.GetUser(User.Identity.Name);

        if (!MembersRepository.IsMember(SERVERID, USERID))
        {
            return new BadRequestObjectResult("Данный пользователь не участник сервера.");
        }
            
        if (!ServersRepository.IsOwner(SERVERID, user.ID) && 
            !RolesRepository.IsUserHavePermission(USERID, SERVERID, "MUTE_TEXT"))
        {
            return new UnauthorizedObjectResult("Нет прав.");
        }
            
        MembersRepository.SetTextMute(USERID, SERVERID, VALUE);
            
        return new OkObjectResult("Пользователь замьючен.");
    }
    
    [HttpPut("voicemute")]
    public ActionResult VoiceMute(int USERID, int SERVERID, bool VALUE)
    {
        User user = AuthRepository.GetUser(User.Identity.Name);

        if (!MembersRepository.IsMember(SERVERID, USERID))
        {
            return new BadRequestObjectResult("Данный пользователь не участник сервера.");
        }
            
        if (!ServersRepository.IsOwner(SERVERID, user.ID) && 
            !RolesRepository.IsUserHavePermission(USERID, SERVERID, "MUTE_VOICE"))
        {
            return new UnauthorizedObjectResult("Нет прав.");
        }
            
        MembersRepository.SetVoiceMute(USERID, SERVERID, VALUE);
            
        return new OkObjectResult("Пользователь замьючен.");
    }
    
    //TODO: Возможность кикать людей, и банить
    [HttpGet("get_byid")]
    public ActionResult<Member> GetMember(int ID)
    {
        Member member = MembersRepository.GetMember(ID);
        return new OkObjectResult(member);
    }
    
    [HttpGet("get_byserverid")]
    public ActionResult<List<Member>> GetMembers(int SERVERID)
    {
        return new OkObjectResult(MembersRepository.GetMembersOfServer(SERVERID));
    }
    
    [HttpGet("get_byuserid")]
    public ActionResult<List<Member>> GetMembersOfUser(int USERID)
    {
        return new OkObjectResult(MembersRepository.GetMembersOfUser(USERID));
    }
}