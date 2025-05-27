using System.ComponentModel.DataAnnotations;

namespace NGMapi.Domain.Entities;

public class Chat
{
    [Required]
    public int ID { get; set; }
    [Required]
    public int SERVERID { get; set; }
    [Required]
    public string NAME { get; set; }
    public string DESCRIPTION { get; set; }
    [Required]
    public string TYPE { get; set; }
    public string ROLEID { get; set; }
}