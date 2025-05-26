using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Domain.Entities;
using NGM_Server_rebuild.Services;
using NGMapi.Infrastructure.Repositories;

namespace NGMapi.API.API_v1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServersController : ControllerBase
{
    [HttpPost("create")]
    public ActionResult Create(string NAME, string? DESCRIPTION, string JOINPASSWORD, bool VISIBLE)
    {
        string username = AuthRepository.TryLogin(Request);
        int userID = AuthRepository.GetUser(username).ID;

        if (ServersRepository.IsServerExists(NAME))
        {
            return BadRequest("Сервер с таким именем уже существует");
        }
            
        ServersRepository.Make(NAME, DESCRIPTION, JOINPASSWORD, VISIBLE, userID);
        MembersRepository.Create(ServersRepository.GetServer(NAME).ID, userID, "Основатель");
        return new OkObjectResult("Сервер создан");
    }
    
    [HttpPatch("name")]
    public ActionResult ChangeName(int ID, string NAME)
    {
        string username = AuthRepository.TryLogin(Request);
        int userID = AuthRepository.GetUser(username).ID;

        if (!RolesRepository.IsUserHavePermission(userID, ID, "ADMIN") &&
            !ServersRepository.IsOwner(ID, userID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }
            
        ServersRepository.ChangeName(ID, NAME);
            
        return new OkObjectResult("Имя сервера измененно");
    }
    
    [HttpPatch("description")]
    public ActionResult ChangeDescription(int ID, string DESCRIPTION)
    {
        string username = AuthRepository.TryLogin(Request);
        int userID = AuthRepository.GetUser(username).ID;
            
        if (!RolesRepository.IsUserHavePermission(userID, ID, "ADMIN") &&
            !ServersRepository.IsOwner(ID, userID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }
            
        ServersRepository.ChangeDescription(ID, DESCRIPTION);
            
        return new OkObjectResult("Описание сервера изменнено");
    }
    
    [HttpPatch("ownerid")]
    public ActionResult ChangeOwner(int ID, int OWNERID)
    {
        string username = AuthRepository.TryLogin(Request);
        int userID = AuthRepository.GetUser(username).ID;

        if (!RolesRepository.IsUserHavePermission(userID, ID, "ADMIN") &&
            !ServersRepository.IsOwner(ID, userID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }
            
        ServersRepository.ChangeOwner(ID, OWNERID);
            
        return new OkObjectResult("Владелец севрера изменён");
    }
    
    [HttpPatch("joinpassword")]
    public ActionResult ChangeJoinPassword(int ID, string JOINPASSWORD)
    {
        string username = AuthRepository.TryLogin(Request);
        int userID = AuthRepository.GetUser(username).ID;

        if (!RolesRepository.IsUserHavePermission(userID, ID, "ADMIN") &&
            !ServersRepository.IsOwner(ID, userID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }
            
        ServersRepository.ChangeJoinPassword(ID, JOINPASSWORD);
            
        return new OkObjectResult("Пароль присоединения изменён");
    }
    
    [HttpPatch("visible")]
    public ActionResult ChangeJoinPassword(int ID, bool VISIBLE)
    {
        string username = AuthRepository.TryLogin(Request);
        int userID = AuthRepository.GetUser(username).ID;

        if (!RolesRepository.IsUserHavePermission(userID, ID, "ADMIN") &&
            !ServersRepository.IsOwner(ID, userID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }
            
        ServersRepository.ChangeVisible(ID, VISIBLE);
            
        return new OkObjectResult("Видимость сервера изменена");
    }
    
    [HttpDelete("delete")]
    public ActionResult Delete(int ID)
    {
        string username = AuthRepository.TryLogin(Request);
        int userID = AuthRepository.GetUser(username).ID;

        if (!ServersRepository.IsOwner(ID, userID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }
            
        ServersRepository.Delete(ID);
        RolesRepository.DeleteAllRolesOfServer(ID);
        MembersRepository.DeleteAllMembersOfServer(ID);
            
        return new OkObjectResult("Сервер удалён");
    }
    
    [HttpGet("get_byname")]
    public ActionResult<Server> GetServer(string NAME)
    {
        Server server = ServersRepository.GetServer(NAME);
        return new OkObjectResult(server);
    }
    
    [HttpGet("get_all")]
    public ActionResult<List<Server>> GetServers()
    {
        return new OkObjectResult(ServersRepository.GetServers());
    }
    
    [HttpGet("get_byid")]
    public ActionResult<Server> GetServerById(string SERVERID)
    {
        return new OkObjectResult(ServersRepository.GetServer(int.Parse(SERVERID)));
    }
}