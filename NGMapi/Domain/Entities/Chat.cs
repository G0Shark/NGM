namespace NGMapi.Domain.Entities;

public class Chat
{
    public int ID { get; set; }
    public int SERVERID { get; set; }
    public string NAME { get; set; }
    public string DESCRIPTION { get; set; }
    public string TYPE { get; set; }
    public string ROLEID { get; set; }
}