using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using NGM_Server_rebuild.Services;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    [HttpPost("api/auth/login")]
    public ActionResult Login()
    {
        try
        {
            AuthService.TryLogin(Request);
            return Ok("Вход успешен");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpGet("api/auth/login")]
    public ActionResult LoginWithUserReturn()
    {
        try
        {
            return new OkObjectResult(AuthService.GetUser(AuthService.TryLogin(Request)));
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpPost("api/auth/register")]
    public ActionResult Register(string USERNAME, string PASSWORD, string EMAIL, string? DISPLAYNAME, string? DESCRIPTION)
    {
        try
        {
            AuthService.Register(USERNAME, PASSWORD, EMAIL, DISPLAYNAME, DESCRIPTION);
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }

        return new ConflictObjectResult("Это сообщения не должно было произойти!\n Произошла неизведанная ошибка, из-за чего бэк вернул 0.\n Обратитесь к G0Shark_ с информацией что ты сделали для получения этой ошибки.");
    }
    
    [HttpDelete("api/auth/delete")]
    public ActionResult DeleteAccount()
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            AuthService.DeleteUser(username);
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
        return new ConflictObjectResult("Это сообщения не должно было произойти!\n Произошла неизведанная ошибка, из-за чего бэк вернул 0.\n Обратитесь к G0Shark_ с информацией что ты сделали для получения этой ошибки.");
    }
    
    [HttpPatch("api/auth/username")]
    public ActionResult UpdateUsername(string NEWUSERNAME)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            AuthService.UpdateUsername(username, NEWUSERNAME);
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
        return new ConflictObjectResult("Это сообщения не должно было произойти!\n Произошла неизведанная ошибка, из-за чего бэк вернул 0.\n Обратитесь к G0Shark_ с информацией что ты сделали для получения этой ошибки.");
    }
    
    [HttpPatch("api/auth/password")]
    public ActionResult UpdatePassword(string PASSWORD)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            AuthService.UpdatePassword(username, PASSWORD);
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
        return new ConflictObjectResult("Это сообщения не должно было произойти!\n Произошла неизведанная ошибка, из-за чего бэк вернул 0.\n Обратитесь к G0Shark_ с информацией что ты сделали для получения этой ошибки.");
    }
    
    [HttpPatch("api/auth/email")]
    public ActionResult UpdateEmail(string EMAIL)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            AuthService.UpdateEmail(username, EMAIL);
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
        return new ConflictObjectResult("Это сообщения не должно было произойти!\n Произошла неизведанная ошибка, из-за чего бэк вернул 0.\n Обратитесь к G0Shark_ с информацией что ты сделали для получения этой ошибки.");
    }
    
    [HttpPatch("api/auth/displayname")]
    public ActionResult UpdateDisplayName(string DISPLAYNAME)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            AuthService.UpdateDisplayName(username, DISPLAYNAME);
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
        return new ConflictObjectResult("Это сообщения не должно было произойти!\n Произошла неизведанная ошибка, из-за чего бэк вернул 0.\n Обратитесь к G0Shark_ с информацией что ты сделали для получения этой ошибки.");
    }
    
    [HttpPatch("api/auth/description")]
    public ActionResult UpdateDescription(string DESCRIPTION)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            AuthService.UpdateDescription(username, DESCRIPTION);
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
        return new ConflictObjectResult("Это сообщения не должно было произойти!\n Произошла неизведанная ошибка, из-за чего бэк вернул 0.\n Обратитесь к G0Shark_ с информацией что ты сделали для получения этой ошибки.");
    }
}