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

            if (!ServersService.IsOwner(SERVERID, ID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }

            if (RolesService.IsLevelExists(SERVERID, LEVEL))
            {
                return new BadRequestObjectResult("Уровень иерархии занят");
            }

            //TODO: Пользователи имеют права создавать роли выше своей, плохо
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

            if (!ServersService.IsOwner(SERVERID, ID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
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

            if (!ServersService.IsOwner(SERVERID, ID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }

            RolesService.ChangeName(ID, NAME);
            return new OkResult();
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

            if (!ServersService.IsOwner(SERVERID, ID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }

            //TODO: Может менять уровень ролям выше своей
            
            RolesService.ChangeLevel(ID, LEVEL);
            return new OkResult();
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

            if (!ServersService.IsOwner(SERVERID, ID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }

            RolesService.ChangePermissions(ID, PERMISSIONS);
            return new OkResult();
        }
        catch (NGMServiceExeption ex)
        {
            return ex.Res;
        }
    }
}