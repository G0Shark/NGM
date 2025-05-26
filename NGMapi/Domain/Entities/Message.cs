namespace NGM_Server_rebuild.Domain.Entities;

public class Message
{
    public int ID { get; set; }
    public int CHATID { get; set; }
    public int SENDERID { get; set; }
    public string MESSAGE { get; set; }
    public int REPLYFORID { get; set; }
    public DateTime TIMESTAMP { get; set; }
    public bool DMCHAT { get; set; }
}