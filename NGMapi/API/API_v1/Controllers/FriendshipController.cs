using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild.Domain.Entities;
using NGMapi.Domain.Interfaces;
using NGMapi.Infrastructure.Repositories;

namespace NGMapi.API.API_v1.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FriendshipController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly IFriendshipRepository _friendshipRepository;

    public FriendshipController(IAuthRepository AUTH_REPOSITORY, IFriendshipRepository FRIENDSHIP_REPOSITORY)
    {
        _authRepository = AUTH_REPOSITORY;
        _friendshipRepository = FRIENDSHIP_REPOSITORY;
    }
    
    [HttpPost("make")]
    public ActionResult MakeFriendShip(int RECEIVERID)
    {
        int SENDERID = _authRepository.GetUser(User.Identity.Name).ID;

        if (SENDERID == RECEIVERID)
        {
            return new BadRequestObjectResult("Нельзя отправить запрос самому себе");
        }
            
        if (!_friendshipRepository.IsFriendshipExists(SENDERID, RECEIVERID) || !_friendshipRepository.IsFriendshipExists(RECEIVERID, SENDERID))
        {
            Friendship f = new Friendship();
            f.SENDERID = SENDERID;
            f.ACCEPTERID = RECEIVERID;
            f.STATE = 0;
            _friendshipRepository.CreateFriendship(f);
            f.SENDERID = RECEIVERID;
            f.ACCEPTERID = SENDERID;
            _friendshipRepository.CreateFriendship(f);

            return new OkObjectResult("Friendship Создан");
        }
        return new BadRequestObjectResult("Friendship уже создан.");
    }
    
    [HttpPost("sendreq")]
    public ActionResult SendFriendRequest(int RECEIVERID)
    {
        int SENDERID = _authRepository.GetUser(User.Identity.Name).ID;
            
        if (!_friendshipRepository.IsFriendshipExists(SENDERID, RECEIVERID) || !_friendshipRepository.IsFriendshipExists(RECEIVERID, SENDERID))
        {
            Friendship f = new Friendship();
            f.SENDERID = SENDERID;
            f.ACCEPTERID = RECEIVERID;
            f.STATE = 0;
            _friendshipRepository.CreateFriendship(f);
            f.SENDERID = RECEIVERID;
            f.ACCEPTERID = SENDERID;
            _friendshipRepository.CreateFriendship(f);
        }

        if (_friendshipRepository.IsFriendRequest(SENDERID, RECEIVERID))
        {
            _friendshipRepository.SetFriendshipState(SENDERID, RECEIVERID, 3);
            _friendshipRepository.SetFriendshipState(RECEIVERID, SENDERID, 3);
                
            return new OkObjectResult("Запрос в друзья принят");
        }

        if (_friendshipRepository.IsFriend(SENDERID, RECEIVERID))
        {
            return new BadRequestObjectResult("Вы уже являетесь друзьями");
        }

        if (_friendshipRepository.IsBlocked(RECEIVERID, SENDERID))
        {
            return new BadRequestObjectResult("Данный пользователь заблокировал вас");
        }
            
        if (_friendshipRepository.IsBlocked(SENDERID, RECEIVERID))
        {
            return new BadRequestObjectResult("Вы заблокировали данного пользователя");
        }
            
        _friendshipRepository.SetFriendshipState(SENDERID, RECEIVERID, 1);
        _friendshipRepository.SetFriendshipState(RECEIVERID, SENDERID, 2);

        return new OkObjectResult("Запрос в друзья отправлен");
    }

    [HttpPost("block")]
    public ActionResult Block(int RECEIVERID)
    {
        int SENDERID = _authRepository.GetUser(User.Identity.Name).ID;
            
        if (!_friendshipRepository.IsFriendshipExists(SENDERID, RECEIVERID) || !_friendshipRepository.IsFriendshipExists(RECEIVERID, SENDERID))
        {
            Friendship f = new Friendship();
            f.SENDERID = SENDERID;
            f.ACCEPTERID = RECEIVERID;
            f.STATE = 0;
            _friendshipRepository.CreateFriendship(f);
            f.SENDERID = RECEIVERID;
            f.ACCEPTERID = SENDERID;
            _friendshipRepository.CreateFriendship(f);
        }
            
        if (_friendshipRepository.IsBlocked(SENDERID, RECEIVERID))
        {
            return new BadRequestObjectResult("Вы уже заблокировали данного пользователя");
        }

        if (_friendshipRepository.IsFriend(RECEIVERID, SENDERID))
        {
            _friendshipRepository.SetFriendshipState(RECEIVERID, SENDERID, 0);
        }
            
        _friendshipRepository.SetFriendshipState(SENDERID, RECEIVERID, 4);

        return new OkObjectResult("Пользователь заблокирован");
    }
    
    [HttpPost("unblock")]
    public ActionResult UnBlock(int RECEIVERID)
    {
        int SENDERID = _authRepository.GetUser(User.Identity.Name).ID;
            
        if (!_friendshipRepository.IsFriendshipExists(SENDERID, RECEIVERID) || !_friendshipRepository.IsFriendshipExists(RECEIVERID, SENDERID))
        {
            Friendship f = new Friendship();
            f.SENDERID = SENDERID;
            f.ACCEPTERID = RECEIVERID;
            f.STATE = 0;
            _friendshipRepository.CreateFriendship(f);
            f.SENDERID = RECEIVERID;
            f.ACCEPTERID = SENDERID;
            _friendshipRepository.CreateFriendship(f);
        }
            
        if (!_friendshipRepository.IsBlocked(SENDERID, RECEIVERID))
        {
            return new BadRequestObjectResult("Пользователь не заблокирован");
        }
            
        _friendshipRepository.SetFriendshipState(SENDERID, RECEIVERID, 0);

        return new OkObjectResult("Пользователь разблокирован");
    }

    [HttpGet("get_byid")]
    public ActionResult<Friendship> GetFriendship(int ID)
    {
        Friendship fs = _friendshipRepository.GetFriendship(ID);
        return new OkObjectResult(fs);
    }
}