namespace lib.Onrealm.Contracts;

public class Roster
{
    public bool? Selected { get; set; }
    public string? RosterId { get; set; }
    public string? IndividualId { get; set; }
    public string? GroupId { get; set; }
    public string? MinistryAreaId { get; set; }
    public string? Label { get; set; }
    public string? PrimaryEmail { get; set; }
    public string? Extension { get; set; }
    public string? PhoneNumber { get; set; }
    public string? RosterType { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? DateAdded { get; set; }
    public object? DateRemoved { get; set; }
    public object? Attributes { get; set; }
    public bool? IsPresent { get; set; }
    public DateTime? BirthDate { get; set; }
    public int? PresentCount { get; set; }
    public int? TotalMarkings { get; set; }
    public bool? CanViewMarkings { get; set; }
    public DateTime? DateLastLoggedIn { get; set; }
    public DateTime? DateLastAttended { get; set; }
    public string? ActionName { get; set; }
    public object? DateDeactivated { get; set; }
    public object? DeceasedMessage { get; set; }
    public string? FamilyLabel { get; set; }
    public int? FamilyPositionType { get; set; }
    public string? Age { get; set; }
}
