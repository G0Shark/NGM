using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Domain.Entities;
using NGMapi.Domain.Interfaces;
using NGMapi.Infrastructure.Repositories;

namespace NGMapi.API.API_v1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly IServersRepository _serversRepository;
    private readonly IRolesRepository _rolesRepository;

    public RolesController(IAuthRepository AUTH_REPOSITORY, IServersRepository SERVERS_REPOSITORY, IRolesRepository ROLES_REPOSITORY)
    {
        _authRepository = AUTH_REPOSITORY;
        _serversRepository = SERVERS_REPOSITORY;
        _rolesRepository = ROLES_REPOSITORY;
    }

    [HttpPost]
    public ActionResult CreateRole(Role role)
    {
        int ID = _authRepository.GetUser(User.Identity.Name).ID;

        if (!_rolesRepository.IsUserHavePermission(ID, role.SERVERID, "CREATE_ROLE") &&
            !_serversRepository.IsOwner(role.SERVERID, ID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }

        if (role.LEVEL < _rolesRepository.UserMaxLevel(ID, role.SERVERID) &&
            !_serversRepository.IsOwner(role.SERVERID, ID))
        {
            return new UnauthorizedObjectResult("Невозможно создать роль выше вашей");
        }

        if (_rolesRepository.IsLevelExists(role.SERVERID, role.LEVEL))
        {
            return new BadRequestObjectResult("Уровень иерархии занят");
        }
            
        _rolesRepository.CreateRole(role);
        return new OkObjectResult("Роль создана");
    }
    
    [HttpDelete]
    public ActionResult DeleteRole(int ID)
    {
        int USERID = _authRepository.GetUser(User.Identity.Name).ID;
        int SERVERID = _rolesRepository.GetServerIDByRoleID(ID);

            
        if (!_rolesRepository.IsUserHavePermission(USERID, SERVERID, "DELETE_ROLE") &&
            !_serversRepository.IsOwner(SERVERID, USERID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }

        if (_rolesRepository.GetLevelOfRole(ID) < _rolesRepository.UserMaxLevel(USERID, SERVERID) &&
            !_serversRepository.IsOwner(SERVERID, USERID))
        {
            return new UnauthorizedObjectResult("Невозможно удалить роль выше вашей");
        }


        _rolesRepository.DeleteRole(ID);
        return new OkObjectResult("Роль удаленна");
    }
    
    [HttpPatch("name")]
    public ActionResult ChangeName(int ID, string NAME)
    {
        int USERID = _authRepository.GetUser(User.Identity.Name).ID;
            
        int SERVERID = _rolesRepository.GetServerIDByRoleID(ID);

        if (!_rolesRepository.IsUserHavePermission(USERID, SERVERID, "CHANGE_ROLE") &&
            !_serversRepository.IsOwner(SERVERID, USERID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }

        if (_rolesRepository.GetLevelOfRole(ID) < _rolesRepository.UserMaxLevel(ID, SERVERID))
        {
            return new UnauthorizedObjectResult("Невозможно изменить роль выше вашей");
        }

        _rolesRepository.ChangeName(ID, NAME);
        return new OkObjectResult("Имя измененно");
    }

    [HttpPatch("level")]
    public ActionResult ChangeLevel(int ID, int LEVEL)
    {
        int USERID = _authRepository.GetUser(User.Identity.Name).ID;
            
        int SERVERID = _rolesRepository.GetServerIDByRoleID(ID);

        if (!_rolesRepository.IsUserHavePermission(USERID, SERVERID, "CHANGE_ROLE") &&
            !_serversRepository.IsOwner(SERVERID, USERID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }

        if (_rolesRepository.GetLevelOfRole(ID) < _rolesRepository.UserMaxLevel(ID, SERVERID))
        {
            return new UnauthorizedObjectResult("Невозможно изменить роль выше вашей");
        }
            
        _rolesRepository.ChangeLevel(ID, LEVEL);
        return new OkObjectResult("Уровень роли изменён");
    }
    
    [HttpPatch("permissions")]
    public ActionResult ChangePermissions(int ID, string PERMISSIONS)
    {
        int USERID = _authRepository.GetUser(User.Identity.Name).ID;
            
        int SERVERID = _rolesRepository.GetServerIDByRoleID(ID);

        if (!_rolesRepository.IsUserHavePermission(USERID, SERVERID, "CHANGE_ROLE") &&
            !_serversRepository.IsOwner(SERVERID, USERID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }

        foreach (var perm in PERMISSIONS.Split(' '))
        {
            if (!_rolesRepository.IsUserHavePermission(USERID, SERVERID, perm))
            {
                return new UnauthorizedObjectResult("У вас нет права, которое вы хотите дать");
            }
        }
            
        if (_rolesRepository.GetLevelOfRole(ID) < _rolesRepository.UserMaxLevel(ID, SERVERID))
        {
            return new UnauthorizedObjectResult("Невозможно изменить роль выше вашей");
        }

        _rolesRepository.ChangePermissions(ID, PERMISSIONS);
        return new OkObjectResult("Права роли изменены");
    }
    
    [HttpGet("get_byid")]
    public ActionResult<Role> GetRole(int ID)
    {
        Role role = _rolesRepository.GetRole(ID);
        return new OkObjectResult(role);
    }
}