namespace NGM_Server_rebuild.Domain.Entities;

public class Role
{
    public int ID { get; set; }
    public int SERVERID { get; set; }
    public int LEVEL { get; set; }
    public string PERMISSIONS { get; set; }
    public string NAME { get; set; }
}