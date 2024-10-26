namespace NGM_Server_rebuild.Models;

public class Message
{
    public string id { get; set; }
    public string chatId { get; set; }
    public string senderId { get; set; }
    public string time { get; set; }
    public string content { get; set; }
    public string answeringMessageId { get; set; }
}