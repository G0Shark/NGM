using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild.Services;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
public class ChatsController : ControllerBase
{
    [HttpPost("api/chats/create")]
    public ActionResult CreateChat(int SERVERID, string NAME, string DESCRIPTION, string TYPE, string ROLEID)
    {
        string username = AuthService.TryLogin(Request);
        int userId = AuthService.GetUser(username).ID;
        var roles = RolesService.GetRolesOfUser(userId, SERVERID);

        if (TYPE == "CHAT")
        {
            if (!RolesService.IsUserHavePermission(userId, SERVERID, "CREATE_CHAT"))
            {
                return new UnauthorizedObjectResult("Нет права создать текстовый чат");
            }
        }
        if (TYPE == "VOICE")
        {
            if (!RolesService.IsUserHavePermission(userId, SERVERID, "CREATE_VOICE"))
            {
                    return new UnauthorizedObjectResult("Нет права создать голосовой чат");
            }
        }

        if (!RolesService.IsUserHavePermission(userId, SERVERID, "ADMIN") || ROLEID == "0")
        {
            foreach (var roleId in ROLEID.Split(' '))
            {
                if (roles.First(x => x.ID == int.Parse(roleId)) != null)
                {
                    return new UnauthorizedObjectResult("Невозможно создать чат в котором нет вашей роли");
                }
            }
        }
        
        ChatsService.MakeChat(SERVERID, NAME, DESCRIPTION, TYPE, ROLEID);
        return new OkObjectResult("Чат создан.");
    }
    
    [HttpDelete("api/chats/delete")]
    public ActionResult DeleteChat(int ID)
    {
        string username = AuthService.TryLogin(Request);
        int userId = AuthService.GetUser(username).ID;
        Chat chat = ChatsService.GetChat(ID);
        var roles = RolesService.GetRolesOfUser(userId, chat.SERVERID);

        if (chat.TYPE == "CHAT")
        {
            if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "DELETE_CHAT"))
            {
                return new UnauthorizedObjectResult("Нет права удалить текстовый чат");
            }
        }
        if (chat.TYPE == "VOICE")
        {
            if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "DELETE_VOICE"))
            {
                return new UnauthorizedObjectResult("Нет права удалить голосовой чат");
            }
        }

        if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "ADMIN") || chat.ROLEID == "0")
        {
            foreach (var roleId in chat.ROLEID.Split(' '))
            {
                if (roles.First(x => x.ID == int.Parse(roleId)) != null)
                {
                    return new UnauthorizedObjectResult("Невозможно удалить чат в котором нет вашей роли");
                }
            }
        }
        
        ChatsService.DeleteChat(ID);
        return new OkObjectResult("Чат удалён.");
    }
    
    [HttpPatch("api/chats/name")]
    public ActionResult ChangeName(int ID, string NAME)
    {
        string username = AuthService.TryLogin(Request);
        int userId = AuthService.GetUser(username).ID;
        Chat chat = ChatsService.GetChat(ID);
        string TYPE = chat.TYPE;
        var roles = RolesService.GetRolesOfUser(userId, chat.SERVERID);

        if (TYPE == "CHAT")
        {
            if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_TEXT"))
            {
                return new UnauthorizedObjectResult("Нет права изменить текстовый чат");
            }
        }
        if (TYPE == "VOICE")
        {
            if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_VOICE"))
            {
                return new UnauthorizedObjectResult("Нет права изменить голосовой чат");
            }
        }
        
        if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "ADMIN") || chat.ROLEID == "0")
        {
            foreach (var roleId in chat.ROLEID.Split(' '))
            {
                if (roles.First(x => x.ID == int.Parse(roleId)) != null)
                {
                    return new UnauthorizedObjectResult("Невозможно изменить чат в котором нет вашей роли");
                }
            }
        }
        
        ChatsService.UpdateName(ID, NAME);
        return new OkObjectResult("Имя чата изменено.");
    }
    
    [HttpPatch("api/chats/description")]
    public ActionResult ChangeDescription(int ID, string DESCRIPTION)
    {
        string username = AuthService.TryLogin(Request);
        int userId = AuthService.GetUser(username).ID;
        Chat chat = ChatsService.GetChat(ID);
        string TYPE = chat.TYPE;
        var roles = RolesService.GetRolesOfUser(userId, chat.SERVERID);

        if (TYPE == "CHAT")
        {
            if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_TEXT"))
            {
                return new UnauthorizedObjectResult("Нет права изменить текстовый чат");
            }
        }
        if (TYPE == "VOICE")
        {
            if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_VOICE"))
            {
                return new UnauthorizedObjectResult("Нет права изменить голосовой чат");
            }
        }
        
        if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "ADMIN") || chat.ROLEID == "0")
        {
            foreach (var roleId in chat.ROLEID.Split(' '))
            {
                if (roles.First(x => x.ID == int.Parse(roleId)) != null)
                {
                    return new UnauthorizedObjectResult("Невозможно изменить чат в котором нет вашей роли");
                }
            }
        }
        
        ChatsService.UpdateDescription(ID, DESCRIPTION);
        return new OkObjectResult("Описание чата изменено.");
    }
    
    [HttpPatch("api/chats/roleid")]
    public ActionResult ChangeType(int ID, string ROLEID)
    {
        string username = AuthService.TryLogin(Request);
        int userId = AuthService.GetUser(username).ID;
        Chat chat = ChatsService.GetChat(ID);
        string oldTYPE = chat.TYPE;
        var roles = RolesService.GetRolesOfUser(userId, chat.SERVERID);

        if (oldTYPE == "CHAT")
        {
            if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_TEXT"))
            {
                return new UnauthorizedObjectResult("Нет права изменить текстовый чат");
            }
        }
        if (oldTYPE == "VOICE")
        {
            if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_VOICE"))
            {
                return new UnauthorizedObjectResult("Нет права изменить голосовой чат");
            }
        }
        
        if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "ADMIN") || chat.ROLEID == "0")
        {
            foreach (var roleId in chat.ROLEID.Split(' '))
            {
                if (roles.First(x => x.ID == int.Parse(roleId)) != null)
                {
                    return new UnauthorizedObjectResult("Невозможно изменить чат в котором нет вашей роли");
                }
            }
            
            foreach (var roleId in ROLEID.Split(' '))
            {
                if (roles.First(x => x.ID == int.Parse(roleId)) != null)
                {
                    return new UnauthorizedObjectResult("Невозможно изменить чат в котором нет вашей роли");
                }
            }
        }
        
        ChatsService.UpdateRoles(ID, ROLEID);
        return new OkObjectResult("Роли чата изменены.");
    }
    
    [HttpGet("api/chats")]
    public ActionResult GetChat(int ID)
    {
        try
        {
            Chat chat = ChatsService.GetChat(ID);
            return new OkObjectResult(chat);
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
}