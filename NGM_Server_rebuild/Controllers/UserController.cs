using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild.Models;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
[Route(Path.MainUserController)]
public class UserController : Controller
{
    [HttpPost(Path.Login)]
    public IActionResult Login([FromBody]User u)
    {
        try
        {
            if (!Secure.CheckUserExists(u.Username))
            {
                return NotFound("Пользователя не существует");
            }

            if (Secure.TryLogin(u.Username, u.PasswordHash))
            {
                return Ok(Secure.GetUser(u.Username, u.PasswordHash));
            }

            return Unauthorized("Логин\\Пароль неверны.");
        }
        catch (Exception ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPost(Path.Register)]
    public IActionResult Register([FromBody] User u)
    {
        try
        {
            if (Secure.CheckUserExists(u.Username))
            {
                return Forbid("Пользователь с таким Username-ом уже существует");
            }
            Secure.CreateUser(u.Username, u.PasswordHash, u.DisplayName, u.Email);
            return Ok(Secure.GetUser(u.Username, u.PasswordHash));
        }
        catch (Exception ex)
        {
            return Conflict(ex.Message);
        }
    }
}