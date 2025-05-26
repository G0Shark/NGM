using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NGM_Server_rebuild.Domain.Entities;
using NGM_Server_rebuild.Services;
using NGMapi.Domain.Entities;
using NGMapi.Infrastructure.Repositories;
using SignalRSwaggerGen.Attributes;

namespace NGM_Server_rebuild.Hubs;

[SignalRHub]
public class ChatHub : Hub
{
    public async Task Delete(string AUTH, int ID, bool DMCHAT)
    {
        if (MessagingRepository.IsMessageExists(ID))
        {
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Сообщения не существует."));
        } 
        
        Message msg = MessagingRepository.GetMessage(ID);
        
        if (!DMCHAT && ChatsRepository.GetChat(msg.CHATID).TYPE != "TEXT")
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
            
            string username = AuthRepository.TryLoginWithAuthStr(AUTH);
            User caller = AuthRepository.GetUser(username);

            if (!MessagingRepository.IsUserAreSender(ID, caller.ID))
            {
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Нет права удалять чужое сообщение."));
            }
            
            if (DMCHAT)
            {
                if (caller.ID > msg.CHATID)
                {
                    MessagingRepository.DeleteMessage(ID);
                    await Clients.Group(caller.ID+"&"+msg.CHATID).SendAsync("ReceiveDeleteMessage", ID);
                }
                else
                {
                    MessagingRepository.DeleteMessage(ID);
                    await Clients.Group(msg.CHATID+"&"+caller.ID).SendAsync("ReceiveDeleteMessage", ID);
                }
            }
            else
            {
                Chat chat = ChatsRepository.GetChat(ID);
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
                    }
                }
                
                MessagingRepository.DeleteMessage(ID);
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
        if (MessagingRepository.IsMessageExists(ID))
        {
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Сообщения не существует."));
        }
        
        Message msg = MessagingRepository.GetMessage(ID);
        
        if (!DMCHAT && ChatsRepository.GetChat(msg.CHATID).TYPE != "TEXT")
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
            
            string username = AuthRepository.TryLoginWithAuthStr(AUTH);
            User caller = AuthRepository.GetUser(username);

            if (MembersRepository.IsTextMuted(ChatsRepository.GetChat(ID).SERVERID, caller.ID))
            {
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Вы замьючены."));
            }
            
            if (MessagingRepository.IsUserAreSender(ID, caller.ID))
            {
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Нет права изменять чужое сообщение."));
            }
            
            if (DMCHAT)
            {
                if (caller.ID > msg.CHATID)
                {
                    MessagingRepository.EditMessage(ID, NEWMESSAGE);
                    await Clients.Group(caller.ID+"&"+msg.CHATID).SendAsync("ReceiveUpdateMessage", ID, NEWMESSAGE);
                }
                else
                {
                    MessagingRepository.EditMessage(ID, NEWMESSAGE);
                    await Clients.Group(msg.CHATID+"&"+caller.ID).SendAsync("ReceiveUpdateMessage", ID, NEWMESSAGE);
                }
            }
            else
            {
                Chat chat = ChatsRepository.GetChat(ID);
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
                    }
                }
                
                MessagingRepository.EditMessage(ID, NEWMESSAGE);
                await Clients.Group(msg.CHATID.ToString()).SendAsync("ReceiveUpdateMessage", ID, NEWMESSAGE);
            }
        }
        catch (NGMServiceExeption e)
        {
            await Clients.Caller.SendAsync("ReceiveError", e.Res);
        }
    }
    
    public async Task Send(string AUTH, int ID, string MESSAGE, int REPLYFORID, bool DMCHAT)
    {
        if (!DMCHAT && !ChatsRepository.IsChatExists(ID))
        {   
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данного чата не существует."));
            return;
        }
        if (!DMCHAT && ChatsRepository.GetChat(ID).TYPE != "TEXT")
        {
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не текстовый."));
            return;
        }
        
        try
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext == null)
            {
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("HttpContext равен null, вы точно добавили Authorization header?"));
                return;
            }
            
            string username = AuthRepository.TryLoginWithAuthStr(AUTH);
            User caller = AuthRepository.GetUser(username);
            
            if (MembersRepository.IsTextMuted(ChatsRepository.GetChat(ID).SERVERID, caller.ID))
            {
                await Clients.Caller.SendAsync("ReceiveError", new UnauthorizedObjectResult("Вы замьючены."));
                return;
            }

            if (DMCHAT)
            {
                if (caller.ID > ID)
                {
                    await Clients.Group(caller.ID + "&" + ID).SendAsync("ReceiveMessage", MessagingRepository.SendMessage(ID, caller.ID, MESSAGE, REPLYFORID, DMCHAT));
                    return;
                }
                else
                {
                    await Clients.Group(ID+"&"+caller.ID).SendAsync("ReceiveMessage", MessagingRepository.SendMessage(ID, caller.ID, MESSAGE, REPLYFORID, DMCHAT));
                    return;
                }
            }
            else
            {
                Chat chat = ChatsRepository.GetChat(ID);
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
                
                Console.WriteLine("gfgfg");
                Message msg = MessagingRepository.SendMessage(ID, caller.ID, MESSAGE, REPLYFORID, DMCHAT);
                Console.WriteLine("gfgfg");
                string jsontype = JsonSerializer.Serialize(msg);
                Console.WriteLine(jsontype);
                Console.WriteLine("gfgfg2");
                await Clients.Group(ID.ToString()).SendAsync("ReceiveMessage", jsontype);
                Console.WriteLine("gfgfg434");
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
        
        if (ChatsRepository.GetChat(ID).TYPE != "TEXT")
        {
            Console.WriteLine("d");
            await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("Данный чат не текстовый."));
        }
        
        var httpContext = Context.GetHttpContext();
            if (httpContext == null)
            {
                Console.WriteLine("dd");
                await Clients.Caller.SendAsync("ReceiveError", new BadRequestObjectResult("HttpContext равен null, вы точно добавили Authorization header?"));
                return;
            }
            
            string username = AuthRepository.TryLoginWithAuthStr(AUTH);
            User caller = AuthRepository.GetUser(username);
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
                Chat chat = ChatsRepository.GetChat(ID);
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
        if (ChatsRepository.GetChat(ID).TYPE != "TEXT")
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
            
            AuthRepository.TryLoginWithAuthStr(AUTH);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, ID.ToString());
            await base.OnDisconnectedAsync(null);
        }
        catch (NGMServiceExeption e)
        {
            await Clients.Caller.SendAsync("ReceiveError", e.Res);
        }
    }
}