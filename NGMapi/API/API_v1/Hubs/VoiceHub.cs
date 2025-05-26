using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NGM_Server_rebuild.Domain.Entities;
using NGM_Server_rebuild.Services;
using NGMapi.Domain.Entities;
using NGMapi.Infrastructure.Repositories;
using SignalRSwaggerGen.Attributes;

namespace NGM_Server_rebuild.Hubs;

[SignalRHub]
public class VoiceHub : Hub
{
    public async Task SendScreen(int ID, byte[] DATA, bool DMCHAT)
    {
        try
        {
            Chat chat = ChatsRepository.GetChat(ID);
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
            
            string username = AuthRepository.TryLogin(httpContext.Request);
            User caller = AuthRepository.GetUser(username);
            if (MembersRepository.IsVoiceMuted(chat.SERVERID, caller.ID))
            {
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Вы замьючены."));
                return;
            }
            
            if (DMCHAT)
            {
                if (caller.ID == ID)
                {
                    await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Личный чат с самим собой невозможен."));
                    return;
                }
                
                if (caller.ID > ID)
                {
                    await Clients.Group(caller.ID+"&"+ID).SendAsync("ReceiveScreen", caller, DATA);
                    return;
                }
                else
                {
                    await Clients.Group(ID+"&"+caller.ID).SendAsync("ReceiveScreen", caller, DATA);
                    return;
                }
            }
            else
            {
                var roles = RolesRepository.GetRolesOfUser(caller.ID, chat.SERVERID);
                if (!RolesRepository.IsUserHavePermission(caller.ID, chat.SERVERID, "ADMIN") && 
                    !ServersRepository.IsOwner(chat.SERVERID, caller.ID) && chat.ROLEID != "0")
                {
                    bool containsRole = false;
                    foreach (var roleId in chat.ROLEID.Split(' '))
                    {
                        if (roles.Contains(RolesRepository.GetRole(int.Parse(roleId))))
                        {
                            containsRole = true;
                        }
                    }

                    if (!containsRole)
                    {
                        await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Нет права к данному чату, т.к. в нём нет вашей роли."));
                        return;
                    }
                }
                
                await Clients.Group(ID.ToString()).SendAsync("ReceiveScreen", caller, DATA);
            }
        }
        catch (NGMServiceExeption e)
        {
            await Clients.Caller.SendAsync("ReceiveError", e.Res);
            return;
        }
    }
    
    public async Task Send(int ID, byte[] DATA, bool DMCHAT)
    {
        try
        {
            Chat chat = ChatsRepository.GetChat(ID);
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
            
            string username = AuthRepository.TryLogin(httpContext.Request);
            User caller = AuthRepository.GetUser(username);
            if (MembersRepository.IsVoiceMuted(chat.SERVERID, caller.ID))
            {
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Вы замьючены."));
                return;
            }
            
            if (DMCHAT)
            {
                if (caller.ID == ID)
                {
                    await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Личный чат с самим собой невозможен."));
                    return;
                }
                
                if (caller.ID > ID)
                {
                    await Clients.Group(caller.ID+"&"+ID).SendAsync("ReceiveVoice", caller, DATA);
                    return;
                }
                else
                {
                    await Clients.Group(ID+"&"+caller.ID).SendAsync("ReceiveVoice", caller, DATA);
                    return;
                }
            }
            else
            {
                var roles = RolesRepository.GetRolesOfUser(caller.ID, chat.SERVERID);
                if (!RolesRepository.IsUserHavePermission(caller.ID, chat.SERVERID, "ADMIN") && 
                    !ServersRepository.IsOwner(chat.SERVERID, caller.ID) && chat.ROLEID != "0")
                {
                    bool containsRole = false;
                    foreach (var roleId in chat.ROLEID.Split(' '))
                    {
                        if (roles.Contains(RolesRepository.GetRole(int.Parse(roleId))))
                        {
                            containsRole = true;
                        }
                    }

                    if (!containsRole)
                    {
                        await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Нет права к данному чату, т.к. в нём нет вашей роли."));
                        return;
                    }
                }
                
                await Clients.Group(ID.ToString()).SendAsync("ReceiveVoice", caller, DATA);
            }
        }
        catch (NGMServiceExeption e)
        {
            await Clients.Caller.SendAsync("ReceiveError", e.Res);
            return;
        }
    }
    
    public async Task Connect(int ID, bool DMCHAT)
    {
        try
        {
            if (!ChatsRepository.IsChatExists(ID))
            {
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не существует."));
                return;
            }

            Chat chat = ChatsRepository.GetChat(ID);
            
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
            
            string username = AuthRepository.TryLogin(httpContext.Request);
            User caller = AuthRepository.GetUser(username);
            if (DMCHAT)
            {
                if (caller.ID == ID)
                {
                    await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Личный чат с самим собой невозможен."));
                    return;
                }
                
                if (caller.ID > ID)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, caller.ID+"&"+ID);
                    await Clients.Group(caller.ID+"&"+ID).SendAsync("ReceiveUserConnect", caller);
                    await base.OnConnectedAsync();
                    return;
                }
                else
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, ID+"&"+caller.ID);
                    await Clients.Group(ID+"&"+caller.ID).SendAsync("ReceiveUserConnect", caller);
                    await base.OnConnectedAsync();
                    return;
                }
            }
            else
            {
                var roles = RolesRepository.GetRolesOfUser(caller.ID, chat.SERVERID);
                if (!RolesRepository.IsUserHavePermission(caller.ID, chat.SERVERID, "ADMIN") && 
                    !ServersRepository.IsOwner(chat.SERVERID, caller.ID) && chat.ROLEID != "0")
                {
                    bool containsRole = false;
                    foreach (var roleId in chat.ROLEID.Split(' '))
                    {
                        if (roles.Contains(RolesRepository.GetRole(int.Parse(roleId))))
                        {
                            containsRole = true;
                        }
                    }

                    if (!containsRole)
                    {
                        await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Нет права к данному чату, т.к. в нём нет вашей роли."));
                        return;
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
            if (!DMCHAT && ChatsRepository.GetChat(ID).TYPE != "VOICE")
            {
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не голосовой."));
            }
            
            var httpContext = Context.GetHttpContext();
            if (httpContext == null)
            {
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("HttpContext равен null, вы точно добавили Authorization header?"));
                return;
            }
            
            string u = AuthRepository.TryLogin(httpContext.Request);
            User caller = AuthRepository.GetUser(u);
            
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