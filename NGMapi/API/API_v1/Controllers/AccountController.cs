using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using NGMapi.Domain.Entities;
using NGMapi.Domain.Interfaces;

namespace NGMapi.API.API_v1.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly IMembersRepository _membersRepository;
    private readonly IFriendshipRepository _friendshipRepository;

    public AccountController(IAuthRepository AUTH_REPOSITORY, IMembersRepository MEMBERS_REPOSITORY,
        IFriendshipRepository FRIENDSHIP_REPOSITORY)
    {
        _authRepository = AUTH_REPOSITORY;
        _membersRepository = MEMBERS_REPOSITORY;
        _friendshipRepository = FRIENDSHIP_REPOSITORY;
    }
    
    [HttpDelete]
    public ActionResult DeleteAccount()
    {
        User u = _authRepository.GetUser(User.Identity.Name);
        _authRepository.DeleteUser(User.Identity.Name);
        _membersRepository.DeleteUser(u.ID);
        _friendshipRepository.DeleteUser(u.ID);
        return Ok("Пользователь удалён");
    }
    
    [HttpPatch("username")]
    public ActionResult UpdateUsername(string USERNAME)
    {
        _authRepository.UpdateUsername(User.Identity.Name, USERNAME);
        return Ok("username пользователя изменён");
    }
    
    [HttpPatch("password")]
    public ActionResult UpdatePassword(string PASSWORD)
    {
        _authRepository.UpdatePassword(User.Identity.Name, PASSWORD);
        return Ok("password пользователя изменён");
    }
    
    [HttpPatch("email")]
    public ActionResult UpdateEmail(string EMAIL)
    {
        _authRepository.UpdateEmail(User.Identity.Name, EMAIL);
        return Ok("email пользователя изменён");
    }
    
    [HttpPatch("displayname")]
    public ActionResult UpdateDisplayName(string DISPLAYNAME)
    {
        _authRepository.UpdateDisplayName(User.Identity.Name, DISPLAYNAME);
        return Ok("displayname пользователя изменён");
    }
    
    [HttpPatch("description")]
    public ActionResult UpdateDescription(string DESCRIPTION)
    {
        _authRepository.UpdateDescription(User.Identity.Name, DESCRIPTION);
        return Ok("description пользователя изменён");
    }

    [HttpGet("get_byusername")]
    public ActionResult<User> GetUser(string USERNAME)
    {
        User u = _authRepository.GetUser(USERNAME);
        return new OkObjectResult(u);
    }
    
    [HttpGet("get_byid")]
    public ActionResult<User> GetUser(int ID)
    {
        User u = _authRepository.GetUser(ID);
        return new OkObjectResult(u);
    }
}