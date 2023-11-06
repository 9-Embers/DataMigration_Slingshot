namespace lib.Onrealm.Contracts;

public class Group
{
    public string? GroupId { get; set; }
    public string? GroupName { get; set; }
    public string? Description { get; set; }
    public string? MediaUrl { get; set; }
    public string? CampusName { get; set; }
    public DateTime? DateDeactivated { get; set; }
    public int? GroupType { get; set; }
    public int? SortOrder { get; set; }
}
