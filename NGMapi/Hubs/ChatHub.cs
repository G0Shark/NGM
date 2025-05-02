using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NGM_Server_rebuild.Services;

namespace NGM_Server_rebuild.Hubs;

public class ChatHub : Hub
{
    public async Task Delete(int ID, bool DMCHAT)
    {
        if (MessagingService.IsMessageExists(ID))
        {
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Сообщения не существует."));
        }
        
        Message msg = MessagingService.GetMessage(ID);
        
        if (!DMCHAT && ChatsService.GetChat(msg.CHATID).TYPE != "TEXT")
        {
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не текстовый."));
        }
        
        try
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext == null)
            {
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("HttpContext равен null, вы точно добавили Authorization header?"));
                return;
            }
            
            string username = AuthService.TryLogin(httpContext.Request);
            User caller = AuthService.GetUser(username);

            if (MessagingService.IsUserAreSender(ID, caller.ID))
            {
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Нет права изменять чужое сообщение."));
            }
            
            if (DMCHAT)
            {
                if (caller.ID > msg.CHATID)
                {
                    await Clients.Group(caller.ID+"&"+msg.CHATID).SendAsync("ReceiveDeleteMessage", ID);
                    MessagingService.DeleteMessage(ID);
                }
                else
                {
                    await Clients.Group(msg.CHATID+"&"+caller.ID).SendAsync("ReceiveDeleteMessage", ID);
                    MessagingService.DeleteMessage(ID);
                }
            }
            else
            {
                var roles = RolesService.GetRolesOfUser(caller.ID, ChatsService.GetChat(msg.CHATID).SERVERID);
                if (ChatsService.GetNeededRolesForChat(msg.CHATID) == "0")
                {
                    await Clients.Group(msg.CHATID.ToString()).SendAsync("ReceiveUpdateMessage", ID);
                    MessagingService.DeleteMessage(ID);
                }
                
                foreach (string role in ChatsService.GetNeededRolesForChat(msg.CHATID).Split(' '))
                {
                    if (roles.First(x => x.ID == int.Parse(role)) != null)
                    {
                        await Clients.Group(msg.CHATID.ToString()).SendAsync("ReceiveUpdateMessage", ID);
                        MessagingService.DeleteMessage(ID);
                    }
                }
                
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Нет права к данному чату."));
            }
        }
        catch (NGMServiceExeption e)
        {
            await Clients.Caller.SendAsync("ReceiveError", e.Res);
        }
    }
    
    public async Task Update(int ID, string NEWMESSAGE, bool DMCHAT)
    {
        if (MessagingService.IsMessageExists(ID))
        {
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Сообщения не существует."));
        }
        
        Message msg = MessagingService.GetMessage(ID);
        
        if (!DMCHAT && ChatsService.GetChat(msg.CHATID).TYPE != "TEXT")
        {
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не текстовый."));
        }
        
        try
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext == null)
            {
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("HttpContext равен null, вы точно добавили Authorization header?"));
                return;
            }
            
            string username = AuthService.TryLogin(httpContext.Request);
            User caller = AuthService.GetUser(username);

            if (MembersService.IsTextMuted(ChatsService.GetChat(ID).SERVERID, caller.ID))
            {
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Вы замьючены."));
            }
            
            if (MessagingService.IsUserAreSender(ID, caller.ID))
            {
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Нет права изменять чужое сообщение."));
            }
            
            if (DMCHAT)
            {
                if (caller.ID > msg.CHATID)
                {
                    await Clients.Group(caller.ID+"&"+msg.CHATID).SendAsync("ReceiveUpdateMessage", ID, NEWMESSAGE);
                    MessagingService.EditMessage(ID, NEWMESSAGE);
                }
                else
                {
                    await Clients.Group(msg.CHATID+"&"+caller.ID).SendAsync("ReceiveUpdateMessage", ID, NEWMESSAGE);
                    MessagingService.EditMessage(ID, NEWMESSAGE);
                }
            }
            else
            {
                var roles = RolesService.GetRolesOfUser(caller.ID, ChatsService.GetChat(msg.CHATID).SERVERID);
                if (ChatsService.GetNeededRolesForChat(msg.CHATID) == "0")
                {
                    await Clients.Group(msg.CHATID.ToString()).SendAsync("ReceiveUpdateMessage", ID, NEWMESSAGE);
                    MessagingService.EditMessage(ID, NEWMESSAGE);
                }
                
                foreach (string role in ChatsService.GetNeededRolesForChat(msg.CHATID).Split(' '))
                {
                    if (roles.First(x => x.ID == int.Parse(role)) != null)
                    {
                        await Clients.Group(msg.CHATID.ToString()).SendAsync("ReceiveUpdateMessage", ID, NEWMESSAGE);
                        MessagingService.EditMessage(ID, NEWMESSAGE);
                    }
                }
                
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Нет права к данному чату."));
            }
        }
        catch (NGMServiceExeption e)
        {
            await Clients.Caller.SendAsync("ReceiveError", e.Res);
        }
    }
    
    public async Task Send(int ID, string MESSAGE, int REPLYFORID, bool DMCHAT)
    {
        if (!DMCHAT && ChatsService.GetChat(ID).TYPE != "TEXT")
        {
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не текстовый."));
        }
        
        try
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext == null)
            {
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("HttpContext равен null, вы точно добавили Authorization header?"));
                return;
            }
            
            string username = AuthService.TryLogin(httpContext.Request);
            User caller = AuthService.GetUser(username);
            
            if (MembersService.IsTextMuted(ChatsService.GetChat(ID).SERVERID, caller.ID))
            {
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Вы замьючены."));
            }

            if (DMCHAT)
            {
                if (caller.ID > ID)
                {
                    await Clients.Group(caller.ID+"&"+ID).SendAsync("ReceiveMessage", caller, MESSAGE);
                    MessagingService.SendMessage(ID, caller.ID, MESSAGE, REPLYFORID, DMCHAT);
                }
                else
                {
                    await Clients.Group(ID+"&"+caller.ID).SendAsync("ReceiveMessage", caller, MESSAGE);
                    MessagingService.SendMessage(ID, caller.ID, MESSAGE, REPLYFORID, DMCHAT);
                }
            }
            else
            {
                var roles = RolesService.GetRolesOfUser(caller.ID, ChatsService.GetChat(ID).SERVERID);
                if (ChatsService.GetNeededRolesForChat(ID) == "0")
                {
                    await Clients.Group(ID.ToString()).SendAsync("ReceiveMessage", caller, MESSAGE);
                    MessagingService.SendMessage(ID, caller.ID, MESSAGE, REPLYFORID, DMCHAT);
                }
                
                foreach (string role in ChatsService.GetNeededRolesForChat(ID).Split(' '))
                {
                    if (roles.First(x => x.ID == int.Parse(role)) != null)
                    {
                        await Clients.Group(ID.ToString()).SendAsync("ReceiveMessage", caller, MESSAGE);
                        MessagingService.SendMessage(ID, caller.ID, MESSAGE, REPLYFORID, DMCHAT);
                    }
                }
                
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Нет права к данному чату."));
            }
        }
        catch (NGMServiceExeption e)
        {
            await Clients.Caller.SendAsync("ReceiveError", e.Res);
        }
    }
    
    public async Task Connect(int ID, bool DMCHAT)
    {
        if (ChatsService.GetChat(ID).TYPE != "TEXT")
        {
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не текстовый."));
        }
        
        try
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext == null)
            {
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("HttpContext равен null, вы точно добавили Authorization header?"));
                return;
            }
            
            string username = AuthService.TryLogin(httpContext.Request);
            User caller = AuthService.GetUser(username);
            if (DMCHAT)
            {
                if (caller.ID > ID)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, caller.ID+"&"+ID);
                    await base.OnConnectedAsync();
                }
                else
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, ID+"&"+caller.ID);
                    await base.OnConnectedAsync();
                }
            }
            else
            {
                var roles = RolesService.GetRolesOfUser(caller.ID, ChatsService.GetChat(ID).SERVERID);
                if (ChatsService.GetNeededRolesForChat(ID) == "0")
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, ID.ToString());
                    await base.OnConnectedAsync();
                }
                
                foreach (string role in ChatsService.GetNeededRolesForChat(ID).Split(' '))
                {
                    if (roles.First(x => x.ID == int.Parse(role)) != null)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, ID.ToString());
                        await base.OnConnectedAsync();
                    }
                }
                
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Нет права к данному чату."));
            }
        }
        catch (NGMServiceExeption e)
        {
            await Clients.Caller.SendAsync("ReceiveError", e.Res);
            return;
        }
    }
    
    public async Task Disconnect(int ID)
    {
        if (ChatsService.GetChat(ID).TYPE != "TEXT")
        {
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не текстовый."));
        }
        
        try
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext == null)
            {
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("HttpContext равен null, вы точно добавили Authorization header?"));
                return;
            }
            
            AuthService.TryLogin(httpContext.Request);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, ID.ToString());
            await base.OnDisconnectedAsync(null);
        }
        catch (NGMServiceExeption e)
        {
            await Clients.Caller.SendAsync("ReceiveError", e.Res);
        }
    }
}