using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild.Services;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
public class MessagingController : ControllerBase
{
    [HttpGet("api/messages/get")]
    public ActionResult GetMessages(int ID, bool DMCHAT)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int userId = AuthService.GetUser(username).ID;
            Chat chat = ChatsService.GetChat(ID);
            
            if (!DMCHAT && !ChatsService.IsChatExists(ID))
            {
                return new NotFoundObjectResult("Данного чата не существует");
            }

            if (DMCHAT)
            {
                return Ok(MessagingService.GetMessagesFromDM(userId, ID));
            }
            else
            {
                var roles = RolesService.GetRolesOfUser(userId, ChatsService.GetChat(ID).SERVERID);
                if (ChatsService.GetNeededRolesForChat(ID) == "0")
                {
                    return Ok(MessagingService.GetMessages(ID, DMCHAT));
                }
                
                if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "ADMIN") && 
                    !ServersService.IsOwner(chat.SERVERID, userId) && chat.ROLEID != "0")
                {
                    bool containsRole = false;
                    foreach (var roleId in chat.ROLEID.Split(' '))
                    {
                        if (roles.Contains(RolesService.GetRole(int.Parse(roleId))))
                        {
                            containsRole = true;
                        }
                    }

                    if (!containsRole)
                    {
                        return new UnauthorizedObjectResult("Нет прав");
                    }
                }
                
                return Ok(MessagingService.GetMessagesFromDM(userId, ID));
            }
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
}