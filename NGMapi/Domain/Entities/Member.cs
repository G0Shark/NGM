using System.ComponentModel.DataAnnotations;

namespace NGM_Server_rebuild.Domain.Entities;

public class Member
{
    [Required]
    [MaxLength(16)]
    public int ID { get; set; }
    [Required]
    [MaxLength(16)]
    public int SERVERID { get; set; }
    [Required]
    [MaxLength(16)]
    public int USERID { get; set; }
    [Required]
    [MaxLength(16)]
    public int ROLEID { get; set; }
    [MaxLength(100)]
    public string DISPLAYNAME { get; set; }
    [Required]
    public bool TEXTMUTE { get; set; }
    [Required]
    public bool VOICEMUTE { get; set; }
}