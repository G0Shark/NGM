using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild.Domain.Entities;
using NGM_Server_rebuild.Services;
using NGMapi.Domain.Entities;
using NGMapi.Infrastructure.Repositories;

namespace NGMapi.API.API_v1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatsController : ControllerBase
{
    [HttpPost]
    public ActionResult CreateChat(int SERVERID, string NAME, string DESCRIPTION, string TYPE, string ROLEID)
    {
        int userId = AuthRepository.GetUser(User.Identity.Name).ID;
        var roles = RolesRepository.GetRolesOfUser(userId, SERVERID);
            
        if (TYPE == "CHAT")
        {
            if (!RolesRepository.IsUserHavePermission(userId, SERVERID, "CREATE_CHAT") &&
                !ServersRepository.IsOwner(SERVERID, userId))
            {
                return new UnauthorizedObjectResult("Нет права создать текстовый чат");
            }
        }
        if (TYPE == "VOICE")
        {
            if (!RolesRepository.IsUserHavePermission(userId, SERVERID, "CREATE_VOICE") &&
                !ServersRepository.IsOwner(SERVERID, userId))
            {
                return new UnauthorizedObjectResult("Нет права создать голосовой чат");
            }
        }

        if (!RolesRepository.IsUserHavePermission(userId, SERVERID, "ADMIN") && 
            !ServersRepository.IsOwner(SERVERID, userId) && ROLEID != "0")
        {
            bool containsRole = false;
            foreach (var roleId in ROLEID.Split(' '))
            {
                if (roles.Contains(RolesRepository.GetRole(int.Parse(roleId))))
                {
                    containsRole = true;
                }
            }

            if (!containsRole)
            {
                return new UnauthorizedObjectResult("Невозможно создать чат в котором нет вашей роли");
            }
        }
        
        ChatsRepository.MakeChat(SERVERID, NAME, DESCRIPTION, TYPE, ROLEID);
        return new OkObjectResult("Чат создан.");
    }
    
    [HttpDelete]
    public ActionResult DeleteChat(int ID)
    {
        int userId = AuthRepository.GetUser(User.Identity.Name).ID;
        Chat chat = ChatsRepository.GetChat(ID);
        var roles = RolesRepository.GetRolesOfUser(userId, chat.SERVERID);

        if (chat.TYPE == "CHAT")
        {
            if (!RolesRepository.IsUserHavePermission(userId, chat.SERVERID, "DELETE_CHAT") &&
                !ServersRepository.IsOwner(chat.SERVERID, userId))
            {
                return new UnauthorizedObjectResult("Нет права удалить текстовый чат");
            }
        }

        if (chat.TYPE == "VOICE")
        {
            if (!RolesRepository.IsUserHavePermission(userId, chat.SERVERID, "DELETE_VOICE") &&
                !ServersRepository.IsOwner(chat.SERVERID, userId))
            {
                return new UnauthorizedObjectResult("Нет права удалить голосовой чат");
            }
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
                return new UnauthorizedObjectResult("Невозможно создать чат в котором нет вашей роли");
            }
        }

        ChatsRepository.DeleteChat(ID);
        return new OkObjectResult("Чат удалён.");
    }
    
    [HttpPatch("name")]
    public ActionResult ChangeName(int ID, string NAME)
    {
        int userId = AuthRepository.GetUser(User.Identity.Name).ID;
        Chat chat = ChatsRepository.GetChat(ID);
        string TYPE = chat.TYPE;
        var roles = RolesRepository.GetRolesOfUser(userId, chat.SERVERID);

        if (TYPE == "CHAT")
        {
            if (!RolesRepository.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_TEXT") &&
                !ServersRepository.IsOwner(chat.SERVERID, userId))
            {
                return new UnauthorizedObjectResult("Нет права изменить текстовый чат");
            }
        }

        if (TYPE == "VOICE")
        {
            if (!RolesRepository.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_VOICE") &&
                !ServersRepository.IsOwner(chat.SERVERID, userId))
            {
                return new UnauthorizedObjectResult("Нет права изменить голосовой чат");
            }
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
                return new UnauthorizedObjectResult("Невозможно создать чат в котором нет вашей роли");
            }
        }

        ChatsRepository.UpdateName(ID, NAME);
        return new OkObjectResult("Имя чата изменено.");
    }
    
    [HttpPatch("description")]
    public ActionResult ChangeDescription(int ID, string DESCRIPTION)
    {
        int userId = AuthRepository.GetUser(User.Identity.Name).ID;
        Chat chat = ChatsRepository.GetChat(ID);
        string TYPE = chat.TYPE;
        var roles = RolesRepository.GetRolesOfUser(userId, chat.SERVERID);

        if (TYPE == "CHAT")
        {
            if (!RolesRepository.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_TEXT") &&
                !ServersRepository.IsOwner(chat.SERVERID, userId))
            {
                return new UnauthorizedObjectResult("Нет права изменить текстовый чат");
            }
        }

        if (TYPE == "VOICE")
        {
            if (!RolesRepository.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_VOICE") &&
                !ServersRepository.IsOwner(chat.SERVERID, userId))
            {
                return new UnauthorizedObjectResult("Нет права изменить голосовой чат");
            }
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
                return new UnauthorizedObjectResult("Невозможно создать чат в котором нет вашей роли");
            }
        }

        ChatsRepository.UpdateDescription(ID, DESCRIPTION);
        return new OkObjectResult("Описание чата изменено.");
    }
    
    [HttpPatch("roleid")]
    public ActionResult ChangeType(int ID, string ROLEID)
    {
        int userId = AuthRepository.GetUser(User.Identity.Name).ID;
        Chat chat = ChatsRepository.GetChat(ID);
        string oldTYPE = chat.TYPE;
        var roles = RolesRepository.GetRolesOfUser(userId, chat.SERVERID);

        if (oldTYPE == "CHAT")
        {
            if (!RolesRepository.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_TEXT") &&
                !ServersRepository.IsOwner(chat.SERVERID, userId))
            {
                return new UnauthorizedObjectResult("Нет права изменить текстовый чат");
            }
        }
        if (oldTYPE == "VOICE")
        {
            if (!RolesRepository.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_VOICE") &&
                !ServersRepository.IsOwner(chat.SERVERID, userId))
            {
                return new UnauthorizedObjectResult("Нет права изменить голосовой чат");
            }
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
                return new UnauthorizedObjectResult("Невозможно создать чат в котором нет вашей роли");
            }
        }
        
        ChatsRepository.UpdateRoles(ID, ROLEID);
        return new OkObjectResult("Роли чата изменены.");
    }
    
    [HttpGet("get_byid")]
    public ActionResult<Chat> GetChat(int ID)
    {
        Chat chat = ChatsRepository.GetChat(ID);
        return new OkObjectResult(chat);
    }
    
    [HttpGet("get_byserverid")]
    public ActionResult<Chat> GetChats(int SERVERID)
    {
        return new OkObjectResult(ChatsRepository.GetChats(SERVERID));
    }
}