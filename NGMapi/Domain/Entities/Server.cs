using System.ComponentModel.DataAnnotations;

namespace NGM_Server_rebuild.Domain.Entities;

public class Server
{
    [Required]
    public int ID { get; set; }
    [Required]
    [MaxLength(30)]
    [MinLength(3)]
    public string NAME { get; set; }
    public string DESCRIPTION { get; set; }
    public string JOINPASSWORD  { get; set; }
    [Required]
    public bool VISIBLE { get; set; }
    [Required]
    public int OWNERID { get; set; }
    public DateTime CREATION_DATE { get; set; }
}