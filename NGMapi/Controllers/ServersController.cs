using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild.Services;
using Org.BouncyCastle.Ocsp;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
public class ServersController : ControllerBase
{
    [HttpPost("api/servers/create")]
    public ActionResult Create(string NAME, string? DESCRIPTION, string JOINPASSWORD, bool VISIBLE)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int userID = AuthService.GetUser(username).ID;

            if (ServersService.IsServerExists(NAME))
            {
                return BadRequest("Сервер с таким именем уже существует");
            }
            
            ServersService.Make(NAME, DESCRIPTION, JOINPASSWORD, VISIBLE, userID);
            MembersService.Create(ServersService.GetServer(NAME).ID, userID, "Основатель");
            return new OkObjectResult("Сервер создан");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpPatch("api/servers/name")]
    public ActionResult ChangeName(int ID, string NAME)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int userID = AuthService.GetUser(username).ID;

            if (!RolesService.IsUserHavePermission(userID, ID, "ADMIN") &&
                !ServersService.IsOwner(ID, userID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }
            
            ServersService.ChangeName(ID, NAME);
            
            return new OkObjectResult("Имя сервера измененно");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpPatch("api/servers/description")]
    public ActionResult ChangeDescription(int ID, string DESCRIPTION)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int userID = AuthService.GetUser(username).ID;
            
            if (!RolesService.IsUserHavePermission(userID, ID, "ADMIN") &&
                !ServersService.IsOwner(ID, userID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }
            
            ServersService.ChangeDescription(ID, DESCRIPTION);
            
            return new OkObjectResult("Описание сервера изменнено");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpPatch("api/servers/ownerid")]
    public ActionResult ChangeOwner(int ID, int OWNERID)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int userID = AuthService.GetUser(username).ID;

            if (!RolesService.IsUserHavePermission(userID, ID, "ADMIN") &&
                !ServersService.IsOwner(ID, userID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }
            
            ServersService.ChangeOwner(ID, OWNERID);
            
            return new OkObjectResult("Владелец севрера изменён");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpPatch("api/servers/joinpassword")]
    public ActionResult ChangeJoinPassword(int ID, string JOINPASSWORD)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int userID = AuthService.GetUser(username).ID;

            if (!RolesService.IsUserHavePermission(userID, ID, "ADMIN") &&
                !ServersService.IsOwner(ID, userID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }
            
            ServersService.ChangeJoinPassword(ID, JOINPASSWORD);
            
            return new OkObjectResult("Пароль присоединения изменён");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpPatch("api/servers/visible")]
    public ActionResult ChangeJoinPassword(int ID, bool VISIBLE)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int userID = AuthService.GetUser(username).ID;

            if (!RolesService.IsUserHavePermission(userID, ID, "ADMIN") &&
                !ServersService.IsOwner(ID, userID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }
            
            ServersService.ChangeVisible(ID, VISIBLE);
            
            return new OkObjectResult("Видимость сервера изменена");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpDelete("api/servers/delete")]
    public ActionResult Delete(int ID)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int userID = AuthService.GetUser(username).ID;

            if (!ServersService.IsOwner(ID, userID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }
            
            ServersService.Delete(ID);
            RolesService.DeleteAllRolesOfServer(ID);
            MembersService.DeleteAllMembersOfServer(ID);
            
            return new OkObjectResult("Сервер удалён");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpGet("api/servers/get_byname")]
    public ActionResult GetServer(string NAME)
    {
        try
        {
            Server server = ServersService.GetServer(NAME);
            return new OkObjectResult(server);
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpGet("api/servers/get_all")]
    public ActionResult GetServers()
    {
        try
        {
            return new OkObjectResult(ServersService.GetServers());
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
}