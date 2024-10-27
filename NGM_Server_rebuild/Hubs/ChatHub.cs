using Microsoft.AspNetCore.SignalR;
using MySqlX.XDevAPI;
using Org.BouncyCastle.Asn1.Ocsp;

namespace NGM_Server_rebuild.Hubs;

public class ChatHub : Hub
{
    public async Task SendToTextChat(string id, string username, string message)
    {
        await Clients.Group(id).SendAsync("ReceiveMessage", username, message);
    }
    
    
    public async Task connect(string id, string username, string password)
    {
        if (!Secure.TryLogin(username, password))
        {
            Clients.Caller.SendAsync("ReceiveError", "Неверный Логин\\Пароль");
            return;
        }

        if (!Permissions.IsChatExist(id))
        {
            Clients.Caller.SendAsync("RecieveError", "Чата не существует");
            return;
        }
        
        if (!Permissions.IsUserInChat(username, password, id))
        {
            Clients.Caller.SendAsync("RecieveError", "Нет доступа");
            return;
        }
        
        await Groups.AddToGroupAsync(Context.ConnectionId, id);
        await base.OnConnectedAsync();
    }
    
    
    public async Task disconnect(string id, string username, string password)
    {
        if (!Secure.TryLogin(username, password))
        {
            Clients.Caller.SendAsync("ReceiveError", "Неверный Логин\\Пароль");
            return;
        }

        if (!Permissions.IsChatExist(id))
        {
            Clients.Caller.SendAsync("RecieveError", "Чата не существует");
            return;
        }
        
        if (!Permissions.IsUserInChat(username, password, id))
        {
            Clients.Caller.SendAsync("RecieveError", "Нет доступа");
            return;
        }
        
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, id);
        await base.OnDisconnectedAsync(null);
    }
}