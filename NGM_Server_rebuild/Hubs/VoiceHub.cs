using Microsoft.AspNetCore.SignalR;

namespace NGM_Server_rebuild.Hubs;

//Войс хаб -
//хаб для SignalR Voice подключений
public class VoiceHub : Hub
{
    public async Task SendToVoiceChat(string id, string uid, byte[] b64)
    {
        Console.WriteLine($"Stable connection with uid: {uid}");
        await Clients.Group(id).SendAsync("ReceiveVoice", uid, b64);
    }
    public async Task connect(string id, string senderid)
    {
        Console.WriteLine($"Connecting to {id}...");
        await Groups.AddToGroupAsync(Context.ConnectionId, id);
        await base.OnConnectedAsync();
    }
    
    
    public async Task disconnect(string id, string senderid)
    {
        Console.WriteLine($"Disconnecting from {Context.ConnectionId}");
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, id);
        await base.OnDisconnectedAsync(null);
    }
}