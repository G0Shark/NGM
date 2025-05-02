using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NGM_Server_rebuild.Services;

namespace NGM_Server_rebuild.Hubs;

public class VoiceHub : Hub
{
    public async Task Send(int ID, byte[] DATA, bool DMCHAT)
    {
        if (!DMCHAT && ChatsService.GetChat(ID).TYPE != "VOICE")
        {
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не голосовой."));
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

            if (MembersService.IsVoiceMuted(ChatsService.GetChat(ID).SERVERID, caller.ID))
            {
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Вы замьючены."));
            }
            
            if (DMCHAT)
            {
                if (caller.ID > ID)
                {
                    await Clients.Group(caller.ID+"&"+ID).SendAsync("ReceiveVoice", caller, DATA);
                }
                else
                {
                    await Clients.Group(caller.ID+"&"+ID).SendAsync("ReceiveVoice", caller, DATA);
                }
            }
            else
            {
                var roles = RolesService.GetRolesOfUser(caller.ID, ChatsService.GetChat(ID).SERVERID);
                if (ChatsService.GetNeededRolesForChat(ID) == "0")
                {
                    await Clients.Group(ID.ToString()).SendAsync("ReceiveVoice", caller, DATA);
                }
                
                foreach (string role in ChatsService.GetNeededRolesForChat(ID).Split(' '))
                {
                    if (roles.First(x => x.ID == int.Parse(role)) != null)
                    {
                        await Clients.Group(ID.ToString()).SendAsync("ReceiveVoice", caller, DATA);
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
        if (ChatsService.GetChat(ID).TYPE != "VOICE")
        {
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не голосовой."));
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