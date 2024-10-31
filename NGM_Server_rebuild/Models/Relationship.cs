namespace NGM_Server_rebuild.Models;

public class Relationship
{
    public string partnershipId { get; set; }
    public string innerUserId { get; set; }
    public string outerUserId { get; set; }
    public PartnershipType status { get; set; }
}