using System.ComponentModel.DataAnnotations;

namespace NGM_Server_rebuild.Domain.Entities;

public class Friendship
{
    [Required]
    public int ID { get; set; }
    [Required]
    public int SENDERID { get; set; }
    [Required]
    public int ACCEPTERID { get; set; }
    [Required]
    public int STATE { get; set; } 
}