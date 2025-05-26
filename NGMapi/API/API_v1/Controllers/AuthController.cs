using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Domain.Entities;
using NGM_Server_rebuild.Services;
using NGMapi.Infrastructure.Repositories;

namespace NGMapi.API.API_v1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public ActionResult Login()
    {
        AuthRepository.TryLogin(Request);
        return Ok("Вход успешен");
    }
    
    [HttpGet("login")]
    public ActionResult<User> LoginWithUserReturn()
    {
        return new OkObjectResult(AuthRepository.GetUser(AuthRepository.TryLogin(Request)));
    }
    
    [HttpPost("register")]
    public ActionResult Register(string USERNAME, string PASSWORD, string EMAIL, string? DISPLAYNAME, string? DESCRIPTION)
    {
        AuthRepository.Register(USERNAME, PASSWORD, EMAIL, DISPLAYNAME, DESCRIPTION);
        return Ok("Пользователь создан");
    }
    
    [HttpDelete("delete")]
    public ActionResult DeleteAccount()
    {
        string username = AuthRepository.TryLogin(Request);
        User u = AuthRepository.GetUser(username);
        AuthRepository.DeleteUser(username);
        MembersRepository.DeleteUser(u.ID);
        FriendshipRepository.DeleteUser(u.ID);
        return Ok("Пользователь удалён");
    }
    
    [HttpPatch("username")]
    public ActionResult UpdateUsername(string USERNAME)
    {
        string username = AuthRepository.TryLogin(Request);
        AuthRepository.UpdateUsername(username, USERNAME);
        return Ok("username пользователя изменён");
    }
    
    [HttpPatch("password")]
    public ActionResult UpdatePassword(string PASSWORD)
    {
        string username = AuthRepository.TryLogin(Request);
        AuthRepository.UpdatePassword(username, PASSWORD);
        return Ok("password пользователя изменён");
    }
    
    [HttpPatch("email")]
    public ActionResult UpdateEmail(string EMAIL)
    {
        string username = AuthRepository.TryLogin(Request);
        AuthRepository.UpdateEmail(username, EMAIL);
        return Ok("email пользователя изменён");
    }
    
    [HttpPatch("displayname")]
    public ActionResult UpdateDisplayName(string DISPLAYNAME)
    {
        string username = AuthRepository.TryLogin(Request);
        AuthRepository.UpdateDisplayName(username, DISPLAYNAME);
        return Ok("displayname пользователя изменён");
    }
    
    [HttpPatch("description")]
    public ActionResult UpdateDescription(string DESCRIPTION)
    {
        string username = AuthRepository.TryLogin(Request);
        AuthRepository.UpdateDescription(username, DESCRIPTION);
        return Ok("description пользователя изменён");
    }

    [HttpGet("get_byusername")]
    public ActionResult<User> GetUser(string USERNAME)
    {
        User u = AuthRepository.GetUser(USERNAME);
        return new OkObjectResult(u);
    }
    
    [HttpGet("get_byid")]
    public ActionResult<User> GetUser(int ID)
    {
        User u = AuthRepository.GetUser(ID);
        return new OkObjectResult(u);
    }
}