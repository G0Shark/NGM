using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Domain.Entities;
using NGM_Server_rebuild.Services;
using NGMapi.Application.Interfaces;
using NGMapi.Domain.Entities;
using NGMapi.Infrastructure.Repositories;

namespace NGMapi.API.API_v1.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    [HttpDelete]
    public ActionResult DeleteAccount()
    {
        User u = AuthRepository.GetUser(User.Identity.Name);
        AuthRepository.DeleteUser(User.Identity.Name);
        MembersRepository.DeleteUser(u.ID);
        FriendshipRepository.DeleteUser(u.ID);
        return Ok("Пользователь удалён");
    }
    
    [HttpPatch("username")]
    public ActionResult UpdateUsername(string USERNAME)
    {
        AuthRepository.UpdateUsername(User.Identity.Name, USERNAME);
        return Ok("username пользователя изменён");
    }
    
    [HttpPatch("password")]
    public ActionResult UpdatePassword(string PASSWORD)
    {
        AuthRepository.UpdatePassword(User.Identity.Name, PASSWORD);
        return Ok("password пользователя изменён");
    }
    
    [HttpPatch("email")]
    public ActionResult UpdateEmail(string EMAIL)
    {
        AuthRepository.UpdateEmail(User.Identity.Name, EMAIL);
        return Ok("email пользователя изменён");
    }
    
    [HttpPatch("displayname")]
    public ActionResult UpdateDisplayName(string DISPLAYNAME)
    {
        AuthRepository.UpdateDisplayName(User.Identity.Name, DISPLAYNAME);
        return Ok("displayname пользователя изменён");
    }
    
    [HttpPatch("description")]
    public ActionResult UpdateDescription(string DESCRIPTION)
    {
        AuthRepository.UpdateDescription(User.Identity.Name, DESCRIPTION);
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