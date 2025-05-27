using System.ComponentModel.DataAnnotations;

namespace NGM_Server_rebuild.Domain.Entities;

public class Member
{
    [Required]
    public int ID { get; set; }
    [Required]
    public int SERVERID { get; set; }
    [Required]
    public int USERID { get; set; }
    [Required]
    public string ROLEID { get; set; }
    [MaxLength(100)]
    public string DISPLAYNAME { get; set; }
    [Required]
    public bool TEXTMUTE { get; set; }
    [Required]
    public bool VOICEMUTE { get; set; }
}