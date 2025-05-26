using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild.Domain.Entities;
using NGM_Server_rebuild.Services;
using NGMapi.Infrastructure.Repositories;

namespace NGMapi.API.API_v1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FriendshipController : ControllerBase
{
    [HttpPost("make")]
    public ActionResult MakeFriendShip(int RECEIVERID)
    {
        string username = AuthRepository.TryLogin(Request);
        int SENDERID = AuthRepository.GetUser(username).ID;

        if (SENDERID == RECEIVERID)
        {
            return new BadRequestObjectResult("Нельзя отправить запрос самому себе");
        }
            
        if (!FriendshipRepository.IsFriendshipExists(SENDERID, RECEIVERID) || !FriendshipRepository.IsFriendshipExists(RECEIVERID, SENDERID))
        {
            FriendshipRepository.MakeFriendship(SENDERID, RECEIVERID, 0);
            FriendshipRepository.MakeFriendship(RECEIVERID, SENDERID, 0);

            return new OkObjectResult("Friendship Создан");
        }
        return new BadRequestObjectResult("Friendship уже создан.");
    }
    
    [HttpPost("sendreq")]
    public ActionResult SendFriendRequest(int RECEIVERID)
    {
        string username = AuthRepository.TryLogin(Request);
        int SENDERID = AuthRepository.GetUser(username).ID;
            
        if (!FriendshipRepository.IsFriendshipExists(SENDERID, RECEIVERID) || !FriendshipRepository.IsFriendshipExists(RECEIVERID, SENDERID))
        {
            FriendshipRepository.MakeFriendship(SENDERID, RECEIVERID, 0);
            FriendshipRepository.MakeFriendship(RECEIVERID, SENDERID, 0);
        }

        if (FriendshipRepository.IsFriendRequest(SENDERID, RECEIVERID))
        {
            FriendshipRepository.FrendshipChange(SENDERID, RECEIVERID, 3);
            FriendshipRepository.FrendshipChange(RECEIVERID, SENDERID, 3);
                
            return new OkObjectResult("Запрос в друзья принят");
        }

        if (FriendshipRepository.IsFriend(SENDERID, RECEIVERID))
        {
            return new BadRequestObjectResult("Вы уже являетесь друзьями");
        }

        if (FriendshipRepository.IsBlocked(RECEIVERID, SENDERID))
        {
            return new BadRequestObjectResult("Данный пользователь заблокировал вас");
        }
            
        if (FriendshipRepository.IsBlocked(SENDERID, RECEIVERID))
        {
            return new BadRequestObjectResult("Вы заблокировали данного пользователя");
        }
            
        FriendshipRepository.FrendshipChange(SENDERID, RECEIVERID, 1);
        FriendshipRepository.FrendshipChange(RECEIVERID, SENDERID, 2);

        return new OkObjectResult("Запрос в друзья отправлен");
    }

    [HttpPost("block")]
    public ActionResult Block(int RECEIVERID)
    {
        string username = AuthRepository.TryLogin(Request);
        int SENDERID = AuthRepository.GetUser(username).ID;
            
        if (!FriendshipRepository.IsFriendshipExists(SENDERID, RECEIVERID) || !FriendshipRepository.IsFriendshipExists(RECEIVERID, SENDERID))
        {
            FriendshipRepository.MakeFriendship(SENDERID, RECEIVERID, 0);
            FriendshipRepository.MakeFriendship(RECEIVERID, SENDERID, 0);
        }
            
        if (FriendshipRepository.IsBlocked(SENDERID, RECEIVERID))
        {
            return new BadRequestObjectResult("Вы уже заблокировали данного пользователя");
        }

        if (FriendshipRepository.IsFriend(RECEIVERID, SENDERID))
        {
            FriendshipRepository.FrendshipChange(RECEIVERID, SENDERID, 0);
        }
            
        FriendshipRepository.FrendshipChange(SENDERID, RECEIVERID, 4);

        return new OkObjectResult("Пользователь заблокирован");
    }
    
    [HttpPost("unblock")]
    public ActionResult UnBlock(int RECEIVERID)
    {
        string username = AuthRepository.TryLogin(Request);
        int SENDERID = AuthRepository.GetUser(username).ID;
            
        if (!FriendshipRepository.IsFriendshipExists(SENDERID, RECEIVERID) || !FriendshipRepository.IsFriendshipExists(RECEIVERID, SENDERID))
        {
            FriendshipRepository.MakeFriendship(SENDERID, RECEIVERID, 0);
            FriendshipRepository.MakeFriendship(RECEIVERID, SENDERID, 0);
        }
            
        if (!FriendshipRepository.IsBlocked(SENDERID, RECEIVERID))
        {
            return new BadRequestObjectResult("Пользователь не заблокирован");
        }
            
        FriendshipRepository.FrendshipChange(SENDERID, RECEIVERID, 0);

        return new OkObjectResult("Пользователь разблокирован");
    }

    [HttpGet("get_byid")]
    public ActionResult<Friendship> GetFriendship(int ID)
    {
        Friendship fs = FriendshipRepository.GetFriendship(ID);
        return new OkObjectResult(fs);
    }
}