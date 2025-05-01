using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild.Services;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
public class RolesController : ControllerBase
{
    [HttpPost("/api/roles/create")]
    public ActionResult CreateRole(int SERVERID, int LEVEL, string PERMISSIONS, string NAME)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int ID = AuthService.GetUser(username).ID;

            if (!RolesService.IsUserHavePermission(ID, SERVERID, "CREATE_ROLE") ||
                !ServersService.IsOwner(ID, SERVERID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }

            if (LEVEL < RolesService.UserMaxLevel(ID, SERVERID))
            {
                return new UnauthorizedObjectResult("Невозможно создать роль выше вашей");
            }

            if (RolesService.IsLevelExists(SERVERID, LEVEL))
            {
                return new BadRequestObjectResult("Уровень иерархии занят");
            }
            
            RolesService.CreateRole(SERVERID, LEVEL, PERMISSIONS, NAME);
            return new OkObjectResult("Роль создана");
        }
        catch (NGMServiceExeption ex)
        {
            return ex.Res;
        }
    }
    
    [HttpDelete("/api/roles/delete")]
    public ActionResult DeleteRole(int ID)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int USERID = AuthService.GetUser(username).ID;
            
            int SERVERID = RolesService.GetServerIDByRoleID(ID);

            if (!RolesService.IsUserHavePermission(USERID, SERVERID, "DELETE_ROLE") ||
                !ServersService.IsOwner(SERVERID, USERID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }

            if (RolesService.GetLevelOfRole(ID) < RolesService.UserMaxLevel(ID, SERVERID))
            {
                return new UnauthorizedObjectResult("Невозможно удалить роль выше вашей");
            }


            RolesService.DeleteRole(ID);
            return new OkObjectResult("Роль создана");
        }
        catch (NGMServiceExeption ex)
        {
            return ex.Res;
        }
    }
    
    [HttpPatch("/api/roles/name")]
    public ActionResult ChangeName(int ID, string NAME)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int USERID = AuthService.GetUser(username).ID;
            
            int SERVERID = RolesService.GetServerIDByRoleID(ID);

            if (!RolesService.IsUserHavePermission(USERID, SERVERID, "CHANGE_ROLE") ||
                !ServersService.IsOwner(SERVERID, USERID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }

            if (RolesService.GetLevelOfRole(ID) < RolesService.UserMaxLevel(ID, SERVERID))
            {
                return new UnauthorizedObjectResult("Невозможно изменить роль выше вашей");
            }

            RolesService.ChangeName(ID, NAME);
            return new OkObjectResult("Имя измененно");
        }
        catch (NGMServiceExeption ex)
        {
            return ex.Res;
        }
    }

    [HttpPatch("/api/roles/level")]
    public ActionResult ChangeLevel(int ID, int LEVEL)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int USERID = AuthService.GetUser(username).ID;
            
            int SERVERID = RolesService.GetServerIDByRoleID(ID);

            if (!RolesService.IsUserHavePermission(USERID, SERVERID, "CHANGE_ROLE") ||
                !ServersService.IsOwner(SERVERID, USERID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }

            if (RolesService.GetLevelOfRole(ID) < RolesService.UserMaxLevel(ID, SERVERID))
            {
                return new UnauthorizedObjectResult("Невозможно изменить роль выше вашей");
            }
            
            RolesService.ChangeLevel(ID, LEVEL);
            return new OkObjectResult("Уровень роли изменён");
        }
        catch (NGMServiceExeption ex)
        {
            return ex.Res;
        }
    }
    
    [HttpPatch("/api/roles/permissions")]
    public ActionResult ChangePermissions(int ID, string PERMISSIONS)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int USERID = AuthService.GetUser(username).ID;
            
            int SERVERID = RolesService.GetServerIDByRoleID(ID);

            if (!RolesService.IsUserHavePermission(USERID, SERVERID, "CHANGE_ROLE") ||
                !ServersService.IsOwner(SERVERID, USERID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }

            foreach (var perm in PERMISSIONS.Split(' '))
            {
                if (!RolesService.IsUserHavePermission(USERID, SERVERID, perm))
                {
                    return new UnauthorizedObjectResult("У вас нет права, которое вы хотите дать");
                }
            }
            
            if (RolesService.GetLevelOfRole(ID) < RolesService.UserMaxLevel(ID, SERVERID))
            {
                return new UnauthorizedObjectResult("Невозможно изменить роль выше вашей");
            }

            RolesService.ChangePermissions(ID, PERMISSIONS);
            return new OkObjectResult("Права роли изменены");
        }
        catch (NGMServiceExeption ex)
        {
            return ex.Res;
        }
    }
}