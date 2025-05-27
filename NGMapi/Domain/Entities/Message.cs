using System.ComponentModel.DataAnnotations;

namespace NGM_Server_rebuild.Domain.Entities;

public class Message
{
    [Required]
    public int ID { get; set; }
    [Required]
    public int CHATID { get; set; }
    [Required]
    public int SENDERID { get; set; }
    [Required]
    public string MESSAGE { get; set; }
    [Required]
    public int REPLYFORID { get; set; }
    public DateTime TIMESTAMP { get; set; }
    [Required]
    public bool DMCHAT { get; set; }
}