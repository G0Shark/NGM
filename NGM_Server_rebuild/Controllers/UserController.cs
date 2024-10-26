using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild.Models;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
[Route("api/auth")]
public class UserController : Controller
{
    [HttpPost("login")]
    public IActionResult Login([FromBody]User u)
    {
        if (!Secure.CheckUserExists(u.Username)) { return BadRequest("Пользователя не существует"); }

        if (Secure.TryLogin(u.Username, u.PasswordHash))
        {
            return Ok(Secure.GetUser(u.Username, u.PasswordHash));
        }
        else
        {
            return BadRequest("Пароль\\Логин неверны");
        }
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] User u)
    {
        Console.WriteLine(u.Username + " | " + u.PasswordHash +" | "+ u.DisplayName+" | "+ u.Email+" | "+ u.id);
        if (Secure.CheckUserExists(u.Username)) { return BadRequest("Пользователь с таким Username-ом уже существует"); }
        Secure.CreateUser(u.Username, u.PasswordHash, u.DisplayName, u.Email);
        return Ok(Secure.GetUser(u.Username, u.PasswordHash));
    }
}