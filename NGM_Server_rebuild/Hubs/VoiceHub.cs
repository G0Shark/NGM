using Microsoft.AspNetCore.SignalR;

namespace NGM_Server_rebuild.Hubs;

//Войс хаб -
//хаб для SignalR Voice подключений
public class VoiceHub : Hub
{
    public async Task SendToVoiceChat(string id, string username, byte[] b64)
    {
        await Clients.Group(id).SendAsync("ReceiveVoice", username, b64);
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
        await Clients.Group(id).SendAsync("ReceiveMessage", "system", $"{username} присоединился к чату");
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
        await Clients.Group(id).SendAsync("ReceiveMessage", "system", $"{username} отключился от чата");
        await base.OnDisconnectedAsync(null);
    }
}