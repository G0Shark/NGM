using System.ComponentModel.DataAnnotations;

namespace NGMapi.Domain.Entities;

public class User
{
    [Required]
    [MaxLength(16)]
    public int ID { get; set; }
    [Required]
    [MaxLength(60)]
    [MinLength(3)]
    public string USERNAME { get; set; }
    [Required]
    public string PASSWORD { get; set; }
    [Required]
    [MaxLength(100)]
    [MinLength(5)]
    [EmailAddress]
    public string EMAIL { get; set; }
    [MaxLength(100)]
    public string DISPLAYNAME { get; set; }
    public DateTime CREATION_DATE { get; set; }
    public string DESCRIPTION { get; set; }
    [MaxLength(300)]
    public string AVATAR_URL { get; set; }
}