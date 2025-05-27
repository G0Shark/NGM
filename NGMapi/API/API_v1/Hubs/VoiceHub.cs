using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NGM_Server_rebuild;
using NGMapi.Domain.Entities;
using NGMapi.Domain.Interfaces;
using NGMapi.Infrastructure.Repositories;
using SignalRSwaggerGen.Attributes;

namespace NGMapi.API.API_v1.Hubs;

[SignalRHub]
[Authorize]
public class VoiceHub : Hub
{
    private readonly IAuthRepository _authRepository;
    private readonly IChatsRepository _chatsRepository;
    private readonly IMembersRepository _membersRepository;
    private readonly IRolesRepository _rolesRepository;
    private readonly IServersRepository _serversRepository;

    public VoiceHub(IAuthRepository AUTH_REPOSITORY, IChatsRepository CHATS_REPOSITORY, IMembersRepository MEMBERS_REPOSITORY, 
        IRolesRepository ROLES_REPOSITORY, IServersRepository SERVERS_REPOSITORY)
    {
        _authRepository = AUTH_REPOSITORY;
        _chatsRepository = CHATS_REPOSITORY;
        _membersRepository = MEMBERS_REPOSITORY;
        _rolesRepository = ROLES_REPOSITORY;
        _serversRepository = SERVERS_REPOSITORY;
    }

    public async Task SendScreen(int ID, byte[] DATA, bool DMCHAT)
    {
        try
        {
            Chat chat = _chatsRepository.GetChat(ID);
            if (!DMCHAT && chat.TYPE != "VOICE")
            {
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не голосовой."));
                return;
            }
            
            string username = Context.User.Identity.Name;
            User caller = _authRepository.GetUser(username);
            if (_membersRepository.IsVoiceMuted(chat.SERVERID, caller.ID))
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
                var roles = _rolesRepository.GetRolesOfUser(caller.ID, chat.SERVERID);
                if (!_rolesRepository.IsUserHavePermission(caller.ID, chat.SERVERID, "ADMIN") && 
                    !_serversRepository.IsOwner(chat.SERVERID, caller.ID) && chat.ROLEID != "0")
                {
                    bool containsRole = false;
                    foreach (var roleId in chat.ROLEID.Split(' '))
                    {
                        if (roles.Contains(_rolesRepository.GetRole(int.Parse(roleId))))
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
            Chat chat = _chatsRepository.GetChat(ID);
            if (!DMCHAT && chat.TYPE != "VOICE")
            {
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не голосовой."));
                return;
            }
            
            string username = Context.User.Identity.Name;
            User caller = _authRepository.GetUser(username);
            if (_membersRepository.IsVoiceMuted(chat.SERVERID, caller.ID))
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
                var roles = _rolesRepository.GetRolesOfUser(caller.ID, chat.SERVERID);
                if (!_rolesRepository.IsUserHavePermission(caller.ID, chat.SERVERID, "ADMIN") && 
                    !_serversRepository.IsOwner(chat.SERVERID, caller.ID) && chat.ROLEID != "0")
                {
                    bool containsRole = false;
                    foreach (var roleId in chat.ROLEID.Split(' '))
                    {
                        if (roles.Contains(_rolesRepository.GetRole(int.Parse(roleId))))
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
            if (!_chatsRepository.IsChatExists(ID))
            {
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не существует."));
                return;
            }

            Chat chat = _chatsRepository.GetChat(ID);
            
            if (!DMCHAT && chat.TYPE != "VOICE")
            {
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не голосовой."));
                return;
            }
            
            string username = Context.User.Identity.Name;
            User caller = _authRepository.GetUser(username);
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
                var roles = _rolesRepository.GetRolesOfUser(caller.ID, chat.SERVERID);
                if (!_rolesRepository.IsUserHavePermission(caller.ID, chat.SERVERID, "ADMIN") && 
                    !_serversRepository.IsOwner(chat.SERVERID, caller.ID) && chat.ROLEID != "0")
                {
                    bool containsRole = false;
                    foreach (var roleId in chat.ROLEID.Split(' '))
                    {
                        if (roles.Contains(_rolesRepository.GetRole(int.Parse(roleId))))
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
            if (!DMCHAT && _chatsRepository.GetChat(ID).TYPE != "VOICE")
            {
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не голосовой."));
            }
            
            string u = Context.User.Identity.Name;
            User caller = _authRepository.GetUser(u);
            
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