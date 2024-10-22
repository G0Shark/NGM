using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Asn1.Ocsp;

namespace NGM_Server_rebuild.Hubs;

public class ChatHub : Hub
{
    public async Task SendToTextChat(string id, string senderid, string message)
    {
        Console.WriteLine($"Sending message from {senderid}: {message}; id: {id}");
        await Clients.Group(id).SendAsync("ReceiveMessage", senderid, message);
    }
    
    
    public async Task connect(string id, string senderid)
    {
        Console.WriteLine($"Connecting to {id}...");
        await Groups.AddToGroupAsync(Context.ConnectionId, id);
        await base.OnConnectedAsync();
        await Clients.Group(id).SendAsync("ReceiveMessage", "system", "к чату присоединился " + senderid);
    }
    
    
    public async Task disconnect(string id, string senderid)
    {
        Console.WriteLine($"Disconnecting from {Context.ConnectionId}");
        
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, id);
        
        await base.OnDisconnectedAsync(null);
        await Clients.Group(id).SendAsync("ReceiveMessage", "system", "из чата ушёл " + senderid);
    }
}