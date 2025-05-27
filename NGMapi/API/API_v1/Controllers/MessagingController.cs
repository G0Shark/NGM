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
public class MessagingController : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Message>> GetMessages(int ID, bool DMCHAT)
    {
        int userId = AuthRepository.GetUser(User.Identity.Name).ID;
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