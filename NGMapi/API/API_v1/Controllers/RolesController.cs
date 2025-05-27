using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Domain.Entities;
using NGM_Server_rebuild.Services;
using NGMapi.Infrastructure.Repositories;

namespace NGMapi.API.API_v1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    [HttpPost]
    public ActionResult CreateRole(int SERVERID, int LEVEL, string PERMISSIONS, string NAME)
    {
        int ID = AuthRepository.GetUser(User.Identity.Name).ID;

        if (!RolesRepository.IsUserHavePermission(ID, SERVERID, "CREATE_ROLE") &&
            !ServersRepository.IsOwner(SERVERID, ID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }

        if (LEVEL < RolesRepository.UserMaxLevel(ID, SERVERID) &&
            !ServersRepository.IsOwner(SERVERID, ID))
        {
            return new UnauthorizedObjectResult("Невозможно создать роль выше вашей");
        }

        if (RolesRepository.IsLevelExists(SERVERID, LEVEL))
        {
            return new BadRequestObjectResult("Уровень иерархии занят");
        }
            
        RolesRepository.CreateRole(SERVERID, LEVEL, PERMISSIONS, NAME);
        return new OkObjectResult("Роль создана");
    }
    
    [HttpDelete]
    public ActionResult DeleteRole(int ID)
    {
        int USERID = AuthRepository.GetUser(User.Identity.Name).ID;
        int SERVERID = RolesRepository.GetServerIDByRoleID(ID);

            
        if (!RolesRepository.IsUserHavePermission(USERID, SERVERID, "DELETE_ROLE") &&
            !ServersRepository.IsOwner(SERVERID, USERID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }

        if (RolesRepository.GetLevelOfRole(ID) < RolesRepository.UserMaxLevel(USERID, SERVERID) &&
            !ServersRepository.IsOwner(SERVERID, USERID))
        {
            return new UnauthorizedObjectResult("Невозможно удалить роль выше вашей");
        }


        RolesRepository.DeleteRole(ID);
        return new OkObjectResult("Роль удаленна");
    }
    
    [HttpPatch("name")]
    public ActionResult ChangeName(int ID, string NAME)
    {
        int USERID = AuthRepository.GetUser(User.Identity.Name).ID;
            
        int SERVERID = RolesRepository.GetServerIDByRoleID(ID);

        if (!RolesRepository.IsUserHavePermission(USERID, SERVERID, "CHANGE_ROLE") &&
            !ServersRepository.IsOwner(SERVERID, USERID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }

        if (RolesRepository.GetLevelOfRole(ID) < RolesRepository.UserMaxLevel(ID, SERVERID))
        {
            return new UnauthorizedObjectResult("Невозможно изменить роль выше вашей");
        }

        RolesRepository.ChangeName(ID, NAME);
        return new OkObjectResult("Имя измененно");
    }

    [HttpPatch("level")]
    public ActionResult ChangeLevel(int ID, int LEVEL)
    {
        int USERID = AuthRepository.GetUser(User.Identity.Name).ID;
            
        int SERVERID = RolesRepository.GetServerIDByRoleID(ID);

        if (!RolesRepository.IsUserHavePermission(USERID, SERVERID, "CHANGE_ROLE") &&
            !ServersRepository.IsOwner(SERVERID, USERID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }

        if (RolesRepository.GetLevelOfRole(ID) < RolesRepository.UserMaxLevel(ID, SERVERID))
        {
            return new UnauthorizedObjectResult("Невозможно изменить роль выше вашей");
        }
            
        RolesRepository.ChangeLevel(ID, LEVEL);
        return new OkObjectResult("Уровень роли изменён");
    }
    
    [HttpPatch("permissions")]
    public ActionResult ChangePermissions(int ID, string PERMISSIONS)
    {
        int USERID = AuthRepository.GetUser(User.Identity.Name).ID;
            
        int SERVERID = RolesRepository.GetServerIDByRoleID(ID);

        if (!RolesRepository.IsUserHavePermission(USERID, SERVERID, "CHANGE_ROLE") &&
            !ServersRepository.IsOwner(SERVERID, USERID))
        {
            return new UnauthorizedObjectResult("Нет доступа");
        }

        foreach (var perm in PERMISSIONS.Split(' '))
        {
            if (!RolesRepository.IsUserHavePermission(USERID, SERVERID, perm))
            {
                return new UnauthorizedObjectResult("У вас нет права, которое вы хотите дать");
            }
        }
            
        if (RolesRepository.GetLevelOfRole(ID) < RolesRepository.UserMaxLevel(ID, SERVERID))
        {
            return new UnauthorizedObjectResult("Невозможно изменить роль выше вашей");
        }

        RolesRepository.ChangePermissions(ID, PERMISSIONS);
        return new OkObjectResult("Права роли изменены");
    }
    
    [HttpGet("get_byid")]
    public ActionResult<Role> GetRole(int ID)
    {
        Role role = RolesRepository.GetRole(ID);
        return new OkObjectResult(role);
    }
}