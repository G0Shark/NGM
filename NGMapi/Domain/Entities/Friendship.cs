namespace NGM_Server_rebuild.Domain.Entities;

public class Friendship
{
    public int ID { get; set; }
    public int SENDERID { get; set; }
    public int ACCEPTERID { get; set; }
    public int STATE { get; set; } 
}