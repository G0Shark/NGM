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
            int ID = AuthService.GetUser(username).ID;

            ServersService.Make(NAME, DESCRIPTION, JOINPASSWORD, VISIBLE, ID);
            
            return new OkObjectResult("Server создан");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpPatch("api/servers/name")]
    public ActionResult ChangeName(int SERVERID, string NAME)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int ID = AuthService.GetUser(username).ID;

            if (!ServersService.IsOwner(SERVERID, ID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }
            
            ServersService.ChangeName(SERVERID, NAME);
            
            return new OkObjectResult("Server готов");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpPatch("api/servers/description")]
    public ActionResult ChangeDescription(int SERVERID, string DESCRIPTION)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int ID = AuthService.GetUser(username).ID;

            if (!ServersService.IsOwner(SERVERID, ID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }
            
            ServersService.ChangeDescription(SERVERID, DESCRIPTION);
            
            return new OkObjectResult("Server готов");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpPatch("api/servers/owner")]
    public ActionResult ChangeOwner(int SERVERID, int OWNERID)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int ID = AuthService.GetUser(username).ID;

            if (!ServersService.IsOwner(SERVERID, ID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }
            
            ServersService.ChangeOwner(SERVERID, OWNERID);
            
            return new OkObjectResult("Server готов");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpPatch("api/servers/joinpassword")]
    public ActionResult ChangeJoinPassword(int SERVERID, string JOINPASSWORD)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int ID = AuthService.GetUser(username).ID;

            if (!ServersService.IsOwner(SERVERID, ID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }
            
            ServersService.ChangeJoinPassword(SERVERID, JOINPASSWORD);
            
            return new OkObjectResult("Server готов");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpPatch("api/servers/visible")]
    public ActionResult ChangeJoinPassword(int SERVERID, bool VISIBLE)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int ID = AuthService.GetUser(username).ID;

            if (!ServersService.IsOwner(SERVERID, ID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }
            
            ServersService.ChangeVisible(SERVERID, VISIBLE);
            
            return new OkObjectResult("Server готов");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpDelete("api/servers/delete")]
    public ActionResult Delete(int SERVERID)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int ID = AuthService.GetUser(username).ID;

            if (!ServersService.IsOwner(SERVERID, ID))
            {
                return new UnauthorizedObjectResult("Нет доступа");
            }
            
            ServersService.Delete(SERVERID);
            
            return new OkObjectResult("Server удалён");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
}