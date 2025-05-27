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
public class MessagingController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly IChatsRepository _chatsRepository;
    private readonly IServersRepository _serversRepository;
    private readonly IMembersRepository _membersRepository;
    private readonly IRolesRepository _rolesRepository;
    private readonly IMessagingRepository _messagingRepository;

    public MessagingController(IAuthRepository AUTH_REPOSITORY, IChatsRepository CHATS_REPOSITORY, IServersRepository SERVERS_REPOSITORY, 
        IMembersRepository MEMBERS_REPOSITORY, IRolesRepository ROLES_REPOSITORY, IMessagingRepository MESSAGING_REPOSITORY)
    {
        _authRepository = AUTH_REPOSITORY;
        _chatsRepository = CHATS_REPOSITORY;
        _serversRepository = SERVERS_REPOSITORY;
        _membersRepository = MEMBERS_REPOSITORY;
        _rolesRepository = ROLES_REPOSITORY;
        _messagingRepository = MESSAGING_REPOSITORY;
    }

    [HttpGet]
    public ActionResult<List<Message>> GetMessages(int ID, bool DMCHAT)
    {
        int userId = _authRepository.GetUser(User.Identity.Name).ID;
        Chat chat = _chatsRepository.GetChat(ID);
            
        if (!DMCHAT && !_chatsRepository.IsChatExists(ID))
        {
            return new NotFoundObjectResult("Данного чата не существует");
        }

        if (DMCHAT)
        {
            return Ok(_messagingRepository.GetMessagesFromDM(userId, ID));
        }
        
        var roles = _rolesRepository.GetRolesOfUser(userId, _chatsRepository.GetChat(ID).SERVERID);
        if (_chatsRepository.GetRolesForChat(ID) == "0")
        {
            return Ok(_messagingRepository.GetMessages(ID, DMCHAT));
        }
                
        if (!_rolesRepository.IsUserHavePermission(userId, chat.SERVERID, "ADMIN") && 
            !_serversRepository.IsOwner(chat.SERVERID, userId) && chat.ROLEID != "0")
        {
            bool containsRole = false;
            foreach (var roleId in chat.ROLEID.Split(' '))
            {
                if (roles.Contains(_rolesRepository.GetRole(int.Parse(roleId))))
                {
                    containsRole = true;
                }
            }

            if (!containsRole)
            {
                return new UnauthorizedObjectResult("Нет прав");
            }
        }
                
        return Ok(_messagingRepository.GetMessagesFromDM(userId, ID));
    }
}