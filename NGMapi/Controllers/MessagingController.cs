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
                
                foreach (string role in ChatsService.GetNeededRolesForChat(ID).Split(' '))
                {
                    if (roles.First(x => x.ID == int.Parse(role)) != null)
                    {
                        return Ok(MessagingService.GetMessages(ID, DMCHAT));
                    }
                }
                
                return new UnauthorizedObjectResult("Нет прав для чтения сообщений");
            }
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
}