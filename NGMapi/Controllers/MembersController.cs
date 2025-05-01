using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild.Services;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
public class MembersController : ControllerBase
{
    [HttpPost("api/members/join")]
    public ActionResult JoinServer(int SERVERID, string? JOINPASSWORD)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            User user = AuthService.GetUser(username);

            if (!ServersService.IsServerExists(SERVERID))
            {
                return new NotFoundObjectResult("Сервера не существует");
            }

            if (ServersService.IsVisible(SERVERID))
            {
                MembersService.Create(SERVERID, user.ID, user.DISPLAYNAME);
            }
            else
            {
                if (ServersService.TryJoinWithPassword(SERVERID, JOINPASSWORD))
                {
                    MembersService.Create(SERVERID, user.ID, user.DISPLAYNAME);
                }
                else
                {
                    return new UnauthorizedObjectResult("Пароль неверный");
                }
            }
            
            return new OkObjectResult("Вы присоеденились.");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpPost("api/members/leave")]
    public ActionResult JoinServer(int SERVERID)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            User user = AuthService.GetUser(username);

            if (!MembersService.IsMember(SERVERID, user.ID))
            {
                return new BadRequestObjectResult("Вы не участник сервера.");
            }
            
            MembersService.Delete(SERVERID, user.ID);
            
            return new OkObjectResult("Вы покинули сервер.");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
}