namespace NGM_Server_rebuild.Domain.Entities;

public class Member
{
    public int ID { get; set; }
    public int SERVERID { get; set; }
    public int USERID { get; set; }
    public int ROLEID { get; set; }
    public string DISPLAYNAME { get; set; }
    public bool TEXTMUTE { get; set; }
    public bool VOICEMUTE { get; set; }
}