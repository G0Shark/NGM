using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NGM_Server_rebuild.Services;

namespace NGM_Server_rebuild.Hubs;

public class VoiceHub : Hub
{
    public async Task Send(int ID, byte[] DATA, bool DMCHAT)
    {
        try
        {
            Chat chat = ChatsService.GetChat(ID);
            if (!DMCHAT && chat.TYPE != "VOICE")
            {
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не голосовой."));
                return;
            }
            
            var httpContext = Context.GetHttpContext();
            if (httpContext == null)
            {
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("HttpContext равен null, вы точно добавили Authorization header?"));
                return;
            }
            
            string username = AuthService.TryLogin(httpContext.Request);
            User caller = AuthService.GetUser(username);
            if (MembersService.IsVoiceMuted(chat.SERVERID, caller.ID))
            {
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Вы замьючены."));
                return;
            }
            
            if (DMCHAT)
            {
                if (caller.ID == ID)
                {
                    await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Личный чат с самим собой невозможен."));
                }
                
                if (caller.ID > ID)
                {
                    await Clients.Group(caller.ID+"&"+ID).SendAsync("ReceiveVoice", caller, DATA);
                    return;
                }
                else
                {
                    await Clients.Group(caller.ID+"&"+ID).SendAsync("ReceiveVoice", caller, DATA);
                    return;
                }
            }
            else
            {
                var roles = RolesService.GetRolesOfUser(caller.ID, chat.SERVERID);
                if (!RolesService.IsUserHavePermission(caller.ID, chat.SERVERID, "ADMIN") && 
                    !ServersService.IsOwner(chat.SERVERID, caller.ID) && chat.ROLEID != "0")
                {
                    bool containsRole = false;
                    foreach (var roleId in chat.ROLEID.Split(' '))
                    {
                        if (roles.Contains(RolesService.GetRole(int.Parse(roleId))))
                        {
                            containsRole = true;
                        }
                    }

                    if (!containsRole)
                    {
                        await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Нет права к данному чату, т.к. в нём нет вашей роли."));
                    }
                }
                
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Нет права к данному чату."));
            }
        }
        catch (NGMServiceExeption e)
        {
            await Clients.Caller.SendAsync("ReceiveError", "bobo sityation");
            return;
        }
    }
    
    public async Task Connect(int ID, bool DMCHAT)
    {
        try
        {
            if (!ChatsService.IsChatExists(ID))
            {
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не существует."));
                return;
            }

            Chat chat = ChatsService.GetChat(ID);
            
            if (chat.TYPE != "VOICE")
            {
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не голосовой."));
                return;
            }
            
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
                if (caller.ID == ID)
                {
                    await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Личный чат с самим собой невозможен."));
                }
                
                if (caller.ID > ID)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, caller.ID+"&"+ID);
                    await base.OnConnectedAsync();
                    return;
                }
                else
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, ID+"&"+caller.ID);
                    await base.OnConnectedAsync();
                    return;
                }
            }
            else
            {
                var roles = RolesService.GetRolesOfUser(caller.ID, chat.SERVERID);
                if (!RolesService.IsUserHavePermission(caller.ID, chat.SERVERID, "ADMIN") && 
                    !ServersService.IsOwner(chat.SERVERID, caller.ID) && chat.ROLEID != "0")
                {
                    bool containsRole = false;
                    foreach (var roleId in chat.ROLEID.Split(' '))
                    {
                        if (roles.Contains(RolesService.GetRole(int.Parse(roleId))))
                        {
                            containsRole = true;
                        }
                    }

                    if (!containsRole)
                    {
                        await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Нет права к данному чату, т.к. в нём нет вашей роли."));
                    }
                }
                
                await Groups.AddToGroupAsync(Context.ConnectionId, ID.ToString());
                await Clients.Caller.SendAsync("ReceiveUserConnect", caller);
                await base.OnConnectedAsync();
            }
        }
        catch (NGMServiceExeption e)
        {
            await Clients.Caller.SendAsync("ReceiveError", e.Res);
            return;
        }
    }
    
    public async Task Disconnect(int ID, bool DMCHAT)
    {
        try
        {
            if (ChatsService.GetChat(ID).TYPE != "VOICE")
            {
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не голосовой."));
            }
            
            var httpContext = Context.GetHttpContext();
            if (httpContext == null)
            {
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("HttpContext равен null, вы точно добавили Authorization header?"));
                return;
            }
            
            string u = AuthService.TryLogin(httpContext.Request);
            User caller = AuthService.GetUser(u);
            
            if (DMCHAT)
            {
                if (caller.ID == ID)
                {
                    await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Личный чат с самим собой невозможен."));
                }
                if (caller.ID > ID)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, caller.ID+"&"+ID);
                    await Clients.Group(ID.ToString()).SendAsync("ReceiveUserDisconnect", caller);
                    await base.OnDisconnectedAsync(null);
                }
                else
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, ID+"&"+caller.ID);
                    await Clients.Group(ID.ToString()).SendAsync("ReceiveUserDisconnect", caller);
                    await base.OnDisconnectedAsync(null);
                }
            }
            else
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, ID.ToString());
                await Clients.Group(ID.ToString()).SendAsync("ReceiveUserDisconnect", caller);
                await base.OnDisconnectedAsync(null);
            }
        }
        catch (NGMServiceExeption e)
        {
            await Clients.Caller.SendAsync("ReceiveError", e.Res);
        }
    }
}