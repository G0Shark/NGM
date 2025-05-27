using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Domain.Entities;
using NGMapi.Domain.Interfaces;
using NGMapi.Infrastructure.Repositories;

namespace NGMapi.API.API_v1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServersController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly IServersRepository _serversRepository;
    private readonly IMembersRepository _membersRepository;
    private readonly IRolesRepository _rolesRepository;

    public ServersController(IAuthRepository AUTH_REPOSITORY, IServersRepository SERVERS_REPOSITORY, 
        IMembersRepository MEMBERS_REPOSITORY, IRolesRepository ROLES_REPOSITORY)
    {
        _authRepository = AUTH_REPOSITORY;
        _serversRepository = SERVERS_REPOSITORY;
        _membersRepository = MEMBERS_REPOSITORY;
        _rolesRepository = ROLES_REPOSITORY;
    }

    [HttpPost]
    public ActionResult Create(Server server)
    {
        int userID = _authRepository.GetUser(User.Identity.Name).ID;

        if (_serversRepository.IsServerExists(server.NAME))
        {
            return BadRequest("Сервер с таким именем уже существует");
        }
            
        _serversRepository.CreateServer(server);
        _membersRepository.CreateMember(_serversRepository.GetServer(server.NAME).ID, userID, "Основатель");
        return new OkObjectResult("Сервер создан");
    }
    
    [HttpPatch("name")]
    public ActionResult ChangeName(int ID, string NAME)
    {
        int userID = _authRepository.GetUser(User.Identity.Name).ID;

        if (!_rolesRepository.IsUserHavePermission(userID, ID, "ADMIN") &&
            !_serversRepository.IsOwner(ID, userID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }
            
        _serversRepository.ChangeName(ID, NAME);
            
        return new OkObjectResult("Имя сервера измененно");
    }
    
    [HttpPatch("description")]
    public ActionResult ChangeDescription(int ID, string DESCRIPTION)
    {
        int userID = _authRepository.GetUser(User.Identity.Name).ID;
            
        if (!_rolesRepository.IsUserHavePermission(userID, ID, "ADMIN") &&
            !_serversRepository.IsOwner(ID, userID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }
            
        _serversRepository.ChangeDescription(ID, DESCRIPTION);
            
        return new OkObjectResult("Описание сервера изменнено");
    }
    
    [HttpPatch("ownerid")]
    public ActionResult ChangeOwner(int ID, int OWNERID)
    {
        int userID = _authRepository.GetUser(User.Identity.Name).ID;

        if (!_rolesRepository.IsUserHavePermission(userID, ID, "ADMIN") &&
            !_serversRepository.IsOwner(ID, userID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }
            
        _serversRepository.ChangeOwner(ID, OWNERID);
            
        return new OkObjectResult("Владелец севрера изменён");
    }
    
    [HttpPatch("joinpassword")]
    public ActionResult ChangeJoinPassword(int ID, string JOINPASSWORD)
    {
        int userID = _authRepository.GetUser(User.Identity.Name).ID;

        if (!_rolesRepository.IsUserHavePermission(userID, ID, "ADMIN") &&
            !_serversRepository.IsOwner(ID, userID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }
            
        _serversRepository.ChangeJoinPassword(ID, JOINPASSWORD);
            
        return new OkObjectResult("Пароль присоединения изменён");
    }
    
    [HttpPatch("visible")]
    public ActionResult ChangeJoinPassword(int ID, bool VISIBLE)
    {
        int userID = _authRepository.GetUser(User.Identity.Name).ID;

        if (!_rolesRepository.IsUserHavePermission(userID, ID, "ADMIN") &&
            !_serversRepository.IsOwner(ID, userID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }
            
        _serversRepository.ChangeVisible(ID, VISIBLE);
            
        return new OkObjectResult("Видимость сервера изменена");
    }
    
    [HttpDelete]
    public ActionResult Delete(int ID)
    {
        int userID = _authRepository.GetUser(User.Identity.Name).ID;

        if (!_serversRepository.IsOwner(ID, userID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }
            
        _serversRepository.DeleteServer(ID);
        _rolesRepository.DeleteAllRolesOfServer(ID);
        _membersRepository.DeleteAllMembersOfServer(ID);
            
        return new OkObjectResult("Сервер удалён");
    }
    
    [HttpGet("get_byname")]
    public ActionResult<Server> GetServer(string NAME)
    {
        Server server = _serversRepository.GetServer(NAME);
        return new OkObjectResult(server);
    }
    
    [HttpGet("get_all")]
    public ActionResult<List<Server>> GetServers()
    {
        return new OkObjectResult(_serversRepository.GetServers());
    }
    
    [HttpGet("get_byid")]
    public ActionResult<Server> GetServerById(string SERVERID)
    {
        return new OkObjectResult(_serversRepository.GetServer(int.Parse(SERVERID)));
    }
}