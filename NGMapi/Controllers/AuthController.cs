using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI.Common;
using NGM_Server_rebuild.Services;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
public class AuthController
{
    [HttpPost("api/auth/login")]
    public ActionResult Login(string USERNAME, string PASSWORD)
    {
        try
        {
            Services.AuthService.Login(USERNAME, PASSWORD);
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }

        return new ConflictObjectResult("Это сообщения не должно было произойти!\n Произошла неизведанная ошибка, из-за чего бэк вернул 0.\n Обратитесь к G0Shark_ с информацией что ты сделали для получения этой ошибки.");
    }
    
    [HttpPost("api/auth/register")]
    public ActionResult Register(string USERNAME, string PASSWORD, string EMAIL, string? DISPLAYNAME, string? DESCRIPTION)
    {
        try
        {
            Services.AuthService.Register(USERNAME, PASSWORD, EMAIL, DISPLAYNAME, DESCRIPTION);
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }

        return new ConflictObjectResult("Это сообщения не должно было произойти!\n Произошла неизведанная ошибка, из-за чего бэк вернул 0.\n Обратитесь к G0Shark_ с информацией что ты сделали для получения этой ошибки.");
    }
}