using System.ComponentModel.DataAnnotations;

namespace NGM_Server_rebuild.Domain.Entities;

public class Friendship
{
    [Required]
    [MaxLength(16)]
    public int ID { get; set; }
    [Required]
    [MaxLength(16)]
    public int SENDERID { get; set; }
    [Required]
    [MaxLength(16)]
    public int ACCEPTERID { get; set; }
    [Required]
    public int STATE { get; set; } 
}