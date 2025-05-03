using System.Net.Http.Headers;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using NGM_Server_rebuild.Services;
using Org.BouncyCastle.Asn1.Ocsp;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
public class FriendshipController : ControllerBase
{
    [HttpPost("api/friendship/make")]
    public ActionResult MakeFriendShip(int RECEIVERID)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int SENDERID = AuthService.GetUser(username).ID;

            if (SENDERID == RECEIVERID)
            {
                return new BadRequestObjectResult("Нельзя отправить запрос самому себе");
            }
            /*
             * if (AuthService.IsUserExists(RECEIVERID))
            {
                return new BadRequestObjectResult("Пользователя не существует");
            }
            */
            
            if (!FriendshipService.IsFriendshipExists(SENDERID, RECEIVERID) || !FriendshipService.IsFriendshipExists(RECEIVERID, SENDERID))
            {
                FriendshipService.MakeFriendship(SENDERID, RECEIVERID, 0);
                FriendshipService.MakeFriendship(RECEIVERID, SENDERID, 0);

                return new OkObjectResult("Friendship Создан");
            }
            else
            {
                return new BadRequestObjectResult("Friendship уже создан.");
            }
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpPost("api/friendship/sendreq")]
    public ActionResult SendFriendRequest(int RECEIVERID)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int SENDERID = AuthService.GetUser(username).ID;
            
            if (!FriendshipService.IsFriendshipExists(SENDERID, RECEIVERID) || !FriendshipService.IsFriendshipExists(RECEIVERID, SENDERID))
            {
                FriendshipService.MakeFriendship(SENDERID, RECEIVERID, 0);
                FriendshipService.MakeFriendship(RECEIVERID, SENDERID, 0);
            }

            if (FriendshipService.IsFriendRequest(SENDERID, RECEIVERID))
            {
                FriendshipService.FrendshipChange(SENDERID, RECEIVERID, 3);
                FriendshipService.FrendshipChange(RECEIVERID, SENDERID, 3);
                
                return new OkObjectResult("Запрос в друзья принят");
            }

            if (FriendshipService.IsFriend(SENDERID, RECEIVERID))
            {
                return new BadRequestObjectResult("Вы уже являетесь друзьями");
            }

            if (FriendshipService.IsBlocked(RECEIVERID, SENDERID))
            {
                return new BadRequestObjectResult("Данный пользователь заблокировал вас");
            }
            
            if (FriendshipService.IsBlocked(SENDERID, RECEIVERID))
            {
                return new BadRequestObjectResult("Вы заблокировали данного пользователя");
            }
            
            FriendshipService.FrendshipChange(SENDERID, RECEIVERID, 1);
            FriendshipService.FrendshipChange(RECEIVERID, SENDERID, 2);

            return new OkObjectResult("Запрос в друзья отправлен");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }

    [HttpPost("api/friendship/block")]
    public ActionResult Block(int RECEIVERID)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int SENDERID = AuthService.GetUser(username).ID;
            
            if (!FriendshipService.IsFriendshipExists(SENDERID, RECEIVERID) || !FriendshipService.IsFriendshipExists(RECEIVERID, SENDERID))
            {
                FriendshipService.MakeFriendship(SENDERID, RECEIVERID, 0);
                FriendshipService.MakeFriendship(RECEIVERID, SENDERID, 0);
            }
            
            if (FriendshipService.IsBlocked(SENDERID, RECEIVERID))
            {
                return new BadRequestObjectResult("Вы уже заблокировали данного пользователя");
            }

            if (FriendshipService.IsFriend(RECEIVERID, SENDERID))
            {
                FriendshipService.FrendshipChange(RECEIVERID, SENDERID, 0);
            }
            
            FriendshipService.FrendshipChange(SENDERID, RECEIVERID, 4);

            return new OkObjectResult("Пользователь заблокирован");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
    
    [HttpPost("api/friendship/unblock")]
    public ActionResult UnBlock(int RECEIVERID)
    {
        try
        {
            string username = AuthService.TryLogin(Request);
            int SENDERID = AuthService.GetUser(username).ID;
            
            if (!FriendshipService.IsFriendshipExists(SENDERID, RECEIVERID) || !FriendshipService.IsFriendshipExists(RECEIVERID, SENDERID))
            {
                FriendshipService.MakeFriendship(SENDERID, RECEIVERID, 0);
                FriendshipService.MakeFriendship(RECEIVERID, SENDERID, 0);
            }
            
            if (!FriendshipService.IsBlocked(SENDERID, RECEIVERID))
            {
                return new BadRequestObjectResult("Пользователь не заблокирован");
            }
            
            FriendshipService.FrendshipChange(SENDERID, RECEIVERID, 0);

            return new OkObjectResult("Пользователь разблокирован");
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }

    [HttpGet("api/friendship/get_byid")]
    public ActionResult GetFriendship(int ID)
    {
        try
        {
            Friendship fs = FriendshipService.GetFriendship(ID);
            return new OkObjectResult(fs);
        }
        catch (NGMServiceExeption e)
        {
            return e.Res;
        }
    }
}