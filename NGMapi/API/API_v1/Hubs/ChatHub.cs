using System.Security.Claims;
using System.Text.Json;
using HotChocolate.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NGM_Server_rebuild;
using NGM_Server_rebuild.Domain.Entities;
using NGMapi.Domain.Entities;
using NGMapi.Domain.Interfaces;
using SignalRSwaggerGen.Attributes;

namespace NGMapi.API.API_v1.Hubs;

[SignalRHub]
[Authorize]
public class ChatHub : Hub
{
    private readonly IMessagingRepository _messagingRepository;
    private readonly IChatsRepository _chatsRepository;
    private readonly IAuthRepository _authRepository;
    private readonly IRolesRepository _rolesRepository;
    private readonly IServersRepository _serversRepository;
    private readonly IMembersRepository _membersRepository;

    public ChatHub(IMessagingRepository MESSAGING_REPOSITORY, IChatsRepository CHATS_REPOSITORY, IAuthRepository AUTH_REPOSITORY, 
        IRolesRepository ROLES_REPOSITORY, IServersRepository SERVERS_REPOSITORY, IMembersRepository MEMBERS_REPOSITORY)
    {
        _messagingRepository = MESSAGING_REPOSITORY;
        _chatsRepository = CHATS_REPOSITORY;
        _authRepository = AUTH_REPOSITORY;
        _rolesRepository = ROLES_REPOSITORY;
        _serversRepository = SERVERS_REPOSITORY;
        _membersRepository = MEMBERS_REPOSITORY;
    }

    public async Task Delete(string AUTH, int ID, bool DMCHAT)
    {
        if (_messagingRepository.IsMessageExists(ID))
        {
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Сообщения не существует."));
        } 
        
        Message msg = _messagingRepository.GetMessage(ID);
        
        if (!DMCHAT && _chatsRepository.GetChat(msg.CHATID).TYPE != "TEXT")
        {
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не текстовый."));
        }
        
        try
        {
            string username = Context.User.Identity.Name;
            User caller = _authRepository.GetUser(username);

            if (!_messagingRepository.IsUserAreSender(ID, caller.ID))
            {
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Нет права удалять чужое сообщение."));
            }
            
            if (DMCHAT)
            {
                if (caller.ID > msg.CHATID)
                {
                    _messagingRepository.DeleteMessage(ID);
                    await Clients.Group(caller.ID+"&"+msg.CHATID).SendAsync("ReceiveDeleteMessage", ID);
                }
                else
                {
                    _messagingRepository.DeleteMessage(ID);
                    await Clients.Group(msg.CHATID+"&"+caller.ID).SendAsync("ReceiveDeleteMessage", ID);
                }
            }
            else
            {
                Chat chat = _chatsRepository.GetChat(ID);
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
                    }
                }
                
                _messagingRepository.DeleteMessage(ID);
                await Clients.Group(msg.CHATID.ToString()).SendAsync("ReceiveDeleteMessage", ID);
            }
        }
        catch (NGMServiceExeption e)
        {
            await Clients.Caller.SendAsync("ReceiveError", e.Res);
        }
    }
    
    public async Task Update(string AUTH, int ID, string NEWMESSAGE, bool DMCHAT)
    {
        if (_messagingRepository.IsMessageExists(ID))
        {
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Сообщения не существует."));
        }
        
        Message msg = _messagingRepository.GetMessage(ID);
        
        if (!DMCHAT && _chatsRepository.GetChat(msg.CHATID).TYPE != "TEXT")
        {
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не текстовый."));
        }
        
        try
        {
            string username = Context.User.Identity.Name;
            User caller = _authRepository.GetUser(username);

            if (_membersRepository.IsTextMuted(_chatsRepository.GetChat(ID).SERVERID, caller.ID))
            {
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Вы замьючены."));
            }
            
            if (_messagingRepository.IsUserAreSender(ID, caller.ID))
            {
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Нет права изменять чужое сообщение."));
            }
            
            if (DMCHAT)
            {
                if (caller.ID > msg.CHATID)
                {
                    _messagingRepository.EditMessage(ID, NEWMESSAGE);
                    await Clients.Group(caller.ID+"&"+msg.CHATID).SendAsync("ReceiveUpdateMessage", ID, NEWMESSAGE);
                }
                else
                {
                    _messagingRepository.EditMessage(ID, NEWMESSAGE);
                    await Clients.Group(msg.CHATID+"&"+caller.ID).SendAsync("ReceiveUpdateMessage", ID, NEWMESSAGE);
                }
            }
            else
            {
                Chat chat = _chatsRepository.GetChat(ID);
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
                    }
                }
                
                _messagingRepository.EditMessage(ID, NEWMESSAGE);
                await Clients.Group(msg.CHATID.ToString()).SendAsync("ReceiveUpdateMessage", ID, NEWMESSAGE);
            }
        }
        catch (NGMServiceExeption e)
        {
            await Clients.Caller.SendAsync("ReceiveError", e.Res);
        }
    }
    
    public async Task Send(Message message)
    {
        if (!message.DMCHAT && !_chatsRepository.IsChatExists(message.CHATID))
        {   
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данного чата не существует."));
            return;
        }
        if (!message.DMCHAT && _chatsRepository.GetChat(message.CHATID).TYPE != "TEXT")
        {
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не текстовый."));
            return;
        }
        
        try
        {
            string username = Context.User.Identity.Name;
            User caller = _authRepository.GetUser(username);
            
            if (_membersRepository.IsTextMuted(_chatsRepository.GetChat(message.CHATID).SERVERID, caller.ID))
            {
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Вы замьючены."));
                return;
            }

            if (message.DMCHAT)
            {
                if (caller.ID > message.SENDERID)
                {
                    await Clients.Group(caller.ID + "&" + message.SENDERID).SendAsync("ReceiveMessage", _messagingRepository.SendMessage(message));
                    return;
                }
                else
                {
                    await Clients.Group(message.SENDERID+"&"+caller.ID).SendAsync("ReceiveMessage", _messagingRepository.SendMessage(message));
                    return;
                }
            }
            else
            {
                Chat chat = _chatsRepository.GetChat(message.CHATID);
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
                
                Message msg = _messagingRepository.SendMessage(message);
                string jsontype = JsonSerializer.Serialize(msg);
                await Clients.Group(message.CHATID.ToString()).SendAsync("ReceiveMessage", jsontype);
                return;
            }
        }
        catch (NGMServiceExeption e)
        {
            await Clients.Caller.SendAsync("ReceiveError", e.Res);
            return;
        }
    }
    
    public async Task Connect(string AUTH, int ID, bool DMCHAT)
    {
        Console.WriteLine(AUTH);
        
        if (_chatsRepository.GetChat(ID).TYPE != "TEXT")
        {
            Console.WriteLine("d");
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не текстовый."));
        }
            string username = Context.User.Identity.Name;
            User caller = _authRepository.GetUser(username);
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
                Chat chat = _chatsRepository.GetChat(ID);
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
                        Console.WriteLine("ddd");
                        await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Нет права к данному чату, т.к. в нём нет вашей роли."));
                    }
                }
                
                await Groups.AddToGroupAsync(Context.ConnectionId, ID.ToString());
                await base.OnConnectedAsync();
            }
    }
    
    public async Task Disconnect(string AUTH, int ID)
    {
        if (_chatsRepository.GetChat(ID).TYPE != "TEXT")
        {
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не текстовый."));
        }
        
        try
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, ID.ToString());
            await base.OnDisconnectedAsync(null);
        }
        catch (NGMServiceExeption e)
        {
            await Clients.Caller.SendAsync("ReceiveError", e.Res);
        }
    }
}