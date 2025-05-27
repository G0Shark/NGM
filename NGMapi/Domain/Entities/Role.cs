using System.ComponentModel.DataAnnotations;

namespace NGM_Server_rebuild.Domain.Entities;

public class Role
{
    [Required]
    public int ID { get; set; }
    [Required]
    public int SERVERID { get; set; }
    [Required]
    public int LEVEL { get; set; }
    public string PERMISSIONS { get; set; }
    [Required]
    [MaxLength(100)]
    public string NAME { get; set; }
}