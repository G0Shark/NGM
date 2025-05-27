using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild.Domain.Entities;
using NGMapi.Domain.Entities;
using NGMapi.Domain.Interfaces;
using NGMapi.Infrastructure.Repositories;

namespace NGMapi.API.API_v1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatsController : ControllerBase
{
    private readonly IServersRepository _serversRepository;
    private readonly IRolesRepository _rolesRepository;
    private readonly IAuthRepository _authRepository;
    private readonly IChatsRepository _chatsRepository;

    public ChatsController(IServersRepository SERVERS_REPOSITORY, IRolesRepository ROLES_REPOSITORY,
        IAuthRepository AUTH_REPOSITORY, IChatsRepository CHATS_REPOSITORY)
    {
        _serversRepository = SERVERS_REPOSITORY;
        _rolesRepository = ROLES_REPOSITORY;
        _authRepository = AUTH_REPOSITORY;
        _chatsRepository = CHATS_REPOSITORY;
    }
    
    [HttpPost]
    public ActionResult CreateChat(Chat chat)
    {
        int userId = _authRepository.GetUser(User.Identity.Name).ID;
        var roles = _rolesRepository.GetRolesOfUser(userId, chat.SERVERID);
            
        if (chat.TYPE == "CHAT")
        {
            if (!_rolesRepository.IsUserHavePermission(userId, chat.SERVERID, "CREATE_CHAT") &&
                !_serversRepository.IsOwner(chat.SERVERID, userId))
            {
                return new UnauthorizedObjectResult("Нет права создать текстовый чат");
            }
        }
        if (chat.TYPE == "VOICE")
        {
            if (!_rolesRepository.IsUserHavePermission(userId, chat.SERVERID, "CREATE_VOICE") &&
                !_serversRepository.IsOwner(chat.SERVERID, userId))
            {
                return new UnauthorizedObjectResult("Нет права создать голосовой чат");
            }
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
                return new UnauthorizedObjectResult("Невозможно создать чат в котором нет вашей роли");
            }
        }
        
        _chatsRepository.CreateChat(chat);
        return new OkObjectResult("Чат создан.");
    }
    
    [HttpDelete]
    public ActionResult DeleteChat(int ID)
    {
        int userId = _authRepository.GetUser(User.Identity.Name).ID;
        Chat chat = _chatsRepository.GetChat(ID);
        var roles = _rolesRepository.GetRolesOfUser(userId, chat.SERVERID);

        if (chat.TYPE == "CHAT")
        {
            if (!_rolesRepository.IsUserHavePermission(userId, chat.SERVERID, "DELETE_CHAT") &&
                !_serversRepository.IsOwner(chat.SERVERID, userId))
            {
                return new UnauthorizedObjectResult("Нет права удалить текстовый чат");
            }
        }

        if (chat.TYPE == "VOICE")
        {
            if (!_rolesRepository.IsUserHavePermission(userId, chat.SERVERID, "DELETE_VOICE") &&
                !_serversRepository.IsOwner(chat.SERVERID, userId))
            {
                return new UnauthorizedObjectResult("Нет права удалить голосовой чат");
            }
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
                return new UnauthorizedObjectResult("Невозможно создать чат в котором нет вашей роли");
            }
        }

        _chatsRepository.DeleteChat(ID);
        return new OkObjectResult("Чат удалён.");
    }
    
    [HttpPatch("name")]
    public ActionResult ChangeName(int ID, string NAME)
    {
        int userId = _authRepository.GetUser(User.Identity.Name).ID;
        Chat chat = _chatsRepository.GetChat(ID);
        string TYPE = chat.TYPE;
        var roles = _rolesRepository.GetRolesOfUser(userId, chat.SERVERID);

        if (TYPE == "CHAT")
        {
            if (!_rolesRepository.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_TEXT") &&
                !_serversRepository.IsOwner(chat.SERVERID, userId))
            {
                return new UnauthorizedObjectResult("Нет права изменить текстовый чат");
            }
        }

        if (TYPE == "VOICE")
        {
            if (!_rolesRepository.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_VOICE") &&
                !_serversRepository.IsOwner(chat.SERVERID, userId))
            {
                return new UnauthorizedObjectResult("Нет права изменить голосовой чат");
            }
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
                return new UnauthorizedObjectResult("Невозможно создать чат в котором нет вашей роли");
            }
        }

        _chatsRepository.UpdateName(ID, NAME);
        return new OkObjectResult("Имя чата изменено.");
    }
    
    [HttpPatch("description")]
    public ActionResult ChangeDescription(int ID, string DESCRIPTION)
    {
        int userId = _authRepository.GetUser(User.Identity.Name).ID;
        Chat chat = _chatsRepository.GetChat(ID);
        string TYPE = chat.TYPE;
        var roles = _rolesRepository.GetRolesOfUser(userId, chat.SERVERID);

        if (TYPE == "CHAT")
        {
            if (!_rolesRepository.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_TEXT") &&
                !_serversRepository.IsOwner(chat.SERVERID, userId))
            {
                return new UnauthorizedObjectResult("Нет права изменить текстовый чат");
            }
        }

        if (TYPE == "VOICE")
        {
            if (!_rolesRepository.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_VOICE") &&
                !_serversRepository.IsOwner(chat.SERVERID, userId))
            {
                return new UnauthorizedObjectResult("Нет права изменить голосовой чат");
            }
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
                return new UnauthorizedObjectResult("Невозможно создать чат в котором нет вашей роли");
            }
        }

        _chatsRepository.UpdateDescription(ID, DESCRIPTION);
        return new OkObjectResult("Описание чата изменено.");
    }
    
    [HttpPatch("roleid")]
    public ActionResult ChangeType(int ID, string ROLEID)
    {
        int userId = _authRepository.GetUser(User.Identity.Name).ID;
        Chat chat = _chatsRepository.GetChat(ID);
        string oldTYPE = chat.TYPE;
        var roles = _rolesRepository.GetRolesOfUser(userId, chat.SERVERID);

        if (oldTYPE == "CHAT")
        {
            if (!_rolesRepository.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_TEXT") &&
                !_serversRepository.IsOwner(chat.SERVERID, userId))
            {
                return new UnauthorizedObjectResult("Нет права изменить текстовый чат");
            }
        }
        if (oldTYPE == "VOICE")
        {
            if (!_rolesRepository.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_VOICE") &&
                !_serversRepository.IsOwner(chat.SERVERID, userId))
            {
                return new UnauthorizedObjectResult("Нет права изменить голосовой чат");
            }
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
                return new UnauthorizedObjectResult("Невозможно создать чат в котором нет вашей роли");
            }
        }
        
        _chatsRepository.UpdateRoles(ID, ROLEID);
        return new OkObjectResult("Роли чата изменены.");
    }
    
    [HttpGet("get_byid")]
    public ActionResult<Chat> GetChat(int ID)
    {
        Chat chat = _chatsRepository.GetChat(ID);
        return new OkObjectResult(chat);
    }
    
    [HttpGet("get_byserverid")]
    public ActionResult<Chat> GetChats(int SERVERID)
    {
        return new OkObjectResult(_chatsRepository.GetChats(SERVERID));
    }
}