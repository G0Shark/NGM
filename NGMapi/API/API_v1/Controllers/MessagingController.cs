using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Domain.Entities;
using NGM_Server_rebuild.Services;
using NGMapi.Domain.Entities;
using NGMapi.Infrastructure.Repositories;

namespace NGMapi.API.API_v1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagingController : ControllerBase
{
    [HttpGet("get")]
    public ActionResult<List<Message>> GetMessages(int ID, bool DMCHAT)
    {
        string username = AuthRepository.TryLogin(Request);
        int userId = AuthRepository.GetUser(username).ID;
        Chat chat = ChatsRepository.GetChat(ID);
            
        if (!DMCHAT && !ChatsRepository.IsChatExists(ID))
        {
            return new NotFoundObjectResult("Данного чата не существует");
        }

        if (DMCHAT)
        {
            return Ok(MessagingRepository.GetMessagesFromDM(userId, ID));
        }
        
        var roles = RolesRepository.GetRolesOfUser(userId, ChatsRepository.GetChat(ID).SERVERID);
        if (ChatsRepository.GetNeededRolesForChat(ID) == "0")
        {
            return Ok(MessagingRepository.GetMessages(ID, DMCHAT));
        }
                
        if (!RolesRepository.IsUserHavePermission(userId, chat.SERVERID, "ADMIN") && 
            !ServersRepository.IsOwner(chat.SERVERID, userId) && chat.ROLEID != "0")
        {
            bool containsRole = false;
            foreach (var roleId in chat.ROLEID.Split(' '))
            {
                if (roles.Contains(RolesRepository.GetRole(int.Parse(roleId))))
                {
                    containsRole = true;
                }
            }

            if (!containsRole)
            {
                return new UnauthorizedObjectResult("Нет прав");
            }
        }
                
        return Ok(MessagingRepository.GetMessagesFromDM(userId, ID));
    }
}