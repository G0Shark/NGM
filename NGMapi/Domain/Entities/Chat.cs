using System.ComponentModel.DataAnnotations;

namespace NGMapi.Domain.Entities;

public class Chat
{
    [Required]
    [MaxLength(16)]
    public int ID { get; set; }
    [Required]
    [MaxLength(16)]
    public int SERVERID { get; set; }
    [Required]
    public string NAME { get; set; }
    public string DESCRIPTION { get; set; }
    [Required]
    public string TYPE { get; set; }
    public string ROLEID { get; set; }
}