using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild.Services;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
public class ChatsController : ControllerBase
{
    [HttpPost("api/chats/create")]
    public ActionResult CreateChat(int SERVERID, string NAME, string DESCRIPTION, string TYPE, string ROLEID)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int userId = AuthService.GetUser(username).ID;
            var roles = RolesService.GetRolesOfUser(userId, SERVERID);
            
            if (TYPE == "CHAT")
            {
                if (!RolesService.IsUserHavePermission(userId, SERVERID, "CREATE_CHAT") &&
                    !ServersService.IsOwner(SERVERID, userId))
                {
                    return new UnauthorizedObjectResult("Нет права создать текстовый чат");
                }
            }
            if (TYPE == "VOICE")
            {
                if (!RolesService.IsUserHavePermission(userId, SERVERID, "CREATE_VOICE") &&
                    !ServersService.IsOwner(SERVERID, userId))
                {
                    return new UnauthorizedObjectResult("Нет права создать голосовой чат");
                }
            }

            if (!RolesService.IsUserHavePermission(userId, SERVERID, "ADMIN") && 
                !ServersService.IsOwner(SERVERID, userId) && ROLEID != "0")
            {
                bool containsRole = false;
                foreach (var roleId in ROLEID.Split(' '))
                {
                    if (roles.Contains(RolesService.GetRole(int.Parse(roleId))))
                    {
                        containsRole = true;
                    }
                }

                if (!containsRole)
                {
                    return new UnauthorizedObjectResult("Невозможно создать чат в котором нет вашей роли");
                }
            }
        
            ChatsService.MakeChat(SERVERID, NAME, DESCRIPTION, TYPE, ROLEID);
            return new OkObjectResult("Чат создан.");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        } 
    }
    
    [HttpDelete("api/chats/delete")]
    public ActionResult DeleteChat(int ID)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int userId = AuthService.GetUser(username).ID;
            Chat chat = ChatsService.GetChat(ID);
            var roles = RolesService.GetRolesOfUser(userId, chat.SERVERID);

            if (chat.TYPE == "CHAT")
            {
                if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "DELETE_CHAT") &&
                    !ServersService.IsOwner(chat.SERVERID, userId))
                {
                    return new UnauthorizedObjectResult("Нет права удалить текстовый чат");
                }
            }

            if (chat.TYPE == "VOICE")
            {
                if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "DELETE_VOICE") &&
                    !ServersService.IsOwner(chat.SERVERID, userId))
                {
                    return new UnauthorizedObjectResult("Нет права удалить голосовой чат");
                }
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
                    return new UnauthorizedObjectResult("Невозможно создать чат в котором нет вашей роли");
                }
            }

            ChatsService.DeleteChat(ID);
            return new OkObjectResult("Чат удалён.");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpPatch("api/chats/name")]
    public ActionResult ChangeName(int ID, string NAME)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int userId = AuthService.GetUser(username).ID;
            Chat chat = ChatsService.GetChat(ID);
            string TYPE = chat.TYPE;
            var roles = RolesService.GetRolesOfUser(userId, chat.SERVERID);

            if (TYPE == "CHAT")
            {
                if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_TEXT") &&
                    !ServersService.IsOwner(chat.SERVERID, userId))
                {
                    return new UnauthorizedObjectResult("Нет права изменить текстовый чат");
                }
            }

            if (TYPE == "VOICE")
            {
                if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_VOICE") &&
                    !ServersService.IsOwner(chat.SERVERID, userId))
                {
                    return new UnauthorizedObjectResult("Нет права изменить голосовой чат");
                }
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
                    return new UnauthorizedObjectResult("Невозможно создать чат в котором нет вашей роли");
                }
            }

            ChatsService.UpdateName(ID, NAME);
            return new OkObjectResult("Имя чата изменено.");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpPatch("api/chats/description")]
    public ActionResult ChangeDescription(int ID, string DESCRIPTION)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int userId = AuthService.GetUser(username).ID;
            Chat chat = ChatsService.GetChat(ID);
            string TYPE = chat.TYPE;
            var roles = RolesService.GetRolesOfUser(userId, chat.SERVERID);

            if (TYPE == "CHAT")
            {
                if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_TEXT") &&
                    !ServersService.IsOwner(chat.SERVERID, userId))
                {
                    return new UnauthorizedObjectResult("Нет права изменить текстовый чат");
                }
            }

            if (TYPE == "VOICE")
            {
                if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_VOICE") &&
                    !ServersService.IsOwner(chat.SERVERID, userId))
                {
                    return new UnauthorizedObjectResult("Нет права изменить голосовой чат");
                }
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
                    return new UnauthorizedObjectResult("Невозможно создать чат в котором нет вашей роли");
                }
            }

            ChatsService.UpdateDescription(ID, DESCRIPTION);
            return new OkObjectResult("Описание чата изменено.");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpPatch("api/chats/roleid")]
    public ActionResult ChangeType(int ID, string ROLEID)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int userId = AuthService.GetUser(username).ID;
            Chat chat = ChatsService.GetChat(ID);
            string oldTYPE = chat.TYPE;
            var roles = RolesService.GetRolesOfUser(userId, chat.SERVERID);

            if (oldTYPE == "CHAT")
            {
                if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_TEXT") &&
                    !ServersService.IsOwner(chat.SERVERID, userId))
                {
                    return new UnauthorizedObjectResult("Нет права изменить текстовый чат");
                }
            }
            if (oldTYPE == "VOICE")
            {
                if (!RolesService.IsUserHavePermission(userId, chat.SERVERID, "UPDATE_VOICE") &&
                    !ServersService.IsOwner(chat.SERVERID, userId))
                {
                    return new UnauthorizedObjectResult("Нет права изменить голосовой чат");
                }
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
                    return new UnauthorizedObjectResult("Невозможно создать чат в котором нет вашей роли");
                }
            }
        
            ChatsService.UpdateRoles(ID, ROLEID);
            return new OkObjectResult("Роли чата изменены.");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpGet("api/chats/get_byid")]
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