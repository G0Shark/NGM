namespace NGM_Server_rebuild.Domain.Entities;

public class Server
{
    public int ID { get; set; }
    public string NAME { get; set; }
    public string DESCRIPTION { get; set; }
    public string JOINPASSWORD  { get; set; }
    public bool VISIBLE { get; set; }
    public int OWNERID { get; set; }
    public DateTime CREATION_DATE { get; set; }
}