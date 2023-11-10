using CsvHelper.Configuration;

namespace lib.Onrealm.Contracts;

public class Individual
{
    public string? IndividualId { get; set; }
    public string? Label { get; set; }
    public string? Title { get; set; }
    public string? FirstName { get; set; }
    public string? PreferredName { get; set; }
    public string? LastName { get; set; }
    public string? Suffix { get; set; }
    public string? PrimaryEmail { get; set; }
    public string? DateMarkedDeceased { get; set; }
    public string? HomePhoneExtension { get; set; }
    public string? HomePhonePrivacy { get; set; }
    public string? MobilePhoneNumber { get; set; }
    public string? MobilePhoneExtension { get; set; }
    public string? MobilePhonePrivacy { get; set; }
    public string? WorkPhoneNumber { get; set; }
    public string? WorkPhoneExtension { get; set; }
    public string? WorkPhonePrivacy { get; set; }
    public string? OtherPhoneNumber { get; set; }
    public string? OtherPhoneExtension { get; set; }
    public string? OtherPhonePrivacy { get; set; }
    public string? BaptismCompleted { get; set; }
    public string? BaptismDate { get; set; }
    public string? VolumePageEntry { get; set; }
    public string? BaptismNotes { get; set; }
    public string? BaptismNotesAddedBy { get; set; }
    public string? BaptismNotesLastModified { get; set; }
    public string? RootedCompleted { get; set; }
    public string? RootedDate { get; set; }
    public string? RootedNotes { get; set; }
    public string? RootedNotesAddedBy { get; set; }
    public string? RootedNotesLastModified { get; set; }
    public string? SalvationRededicatedLifeCompleted { get; set; }
    public string? SalvationRededicatedLifeDate { get; set; }
    public string? SalvationRededicatedLifeNotes { get; set; }
    public string? SalvationRededicatedLifeNotesAddedBy { get; set; }
    public string? SalvationRededicatedLifeNotesLastModified { get; set; }
    public string? MarriageCompleted { get; set; }
    public string? MarriageDate { get; set; }
    public string? MarriageNotes { get; set; }
    public string? MarriageNotesAddedBy { get; set; }
    public string? MarriageNotesLastModified { get; set; }
    public string? BibleMilestoneCompleted { get; set; }
    public string? BibleMilestoneDate { get; set; }
    public string? BibleMilestoneNotes { get; set; }
    public string? BibleMilestoneNotesAddedBy { get; set; }
    public string? BibleMilestoneNotesLastModified { get; set; }
    public string? ChildDedicationCompleted { get; set; }
    public string? ChildDedicationDate { get; set; }
    public string? ChildDedicationNotes { get; set; }
    public string? ChildDedicationNotesAddedBy { get; set; }
    public string? ChildDedicationNotesLastModified { get; set; }
    public string? FamilyId { get; set; }
    public string? PrimaryPhoneNumber { get; set; }
    public string? PrimaryPhoneNumberExtension { get; set; }
    public string? MailingAddress1 { get; set; }
    public string? MailingAddress2 { get; set; }
    public string? MailingCity { get; set; }
    public string? MailingRegion { get; set; }
    public string? MailingPostalCode { get; set; }
    public string? MailingCountry { get; set; }
    public string? Campus { get; set; }
    public string? IndividualStatus { get; set; }
    public string? DateOfDeath { get; set; }
    public string? MiddleName { get; set; }
    public string? HomePhoneNumber { get; set; }
    public string? GiftDatefirst { get; set; }
    public string? GiftDatelast { get; set; }
    public string? SharedGiving { get; set; }
    public string? GivingNumber { get; set; }
    public DateTime? AddedWhen { get; set; }
    public string? AnniversaryDate { get; set; }
    public string? AllergiesMedicalConcerns { get; set; }
    public string? DateofBirth { get; set; }
    public int? DateofBirthDay { get; set; }
    public int? DateofBirthMonth { get; set; }
    public int? DateofBirthYear { get; set; }
    public string? DoNotEmail { get; set; }
    public string? Gender { get; set; }
    public string? HSGraduationYear { get; set; }
    public string? InsuranceCarrier { get; set; }
    public string? InsurancePolicyNumber { get; set; }
    public string? LastBackgroundCheckDate { get; set; }
    public string? LastBackgroundCheckPackageName { get; set; }
    public string? LastBackgroundCheckStatus { get; set; }
    public string? MaidenName { get; set; }
    public string? MandatedReporterCompletion { get; set; }
    public string? MandatedReporterExpiration { get; set; }
    public string? MaritalStatus { get; set; }
    public string? MemberStatus { get; set; }
    public string? RootedGraduationDate { get; set; }
    public string? NoRegretsGoldGraduate { get; set; }
    public string? NoRegretsSilvergraduate { get; set; }
}

public class IndividualClassMap : CsvClassMap<Individual>
{
    public IndividualClassMap()
    {
        Map( m => m.IndividualId ).Name( "Individual Id" );
        Map( m => m.Label ).Name( "Label" );
        Map( m => m.Title ).Name( "Title" );
        Map( m => m.FirstName ).Name( "First Name" );
        Map( m => m.PreferredName ).Name( "Preferred Name" );
        Map( m => m.LastName ).Name( "Last Name" );
        Map( m => m.Suffix ).Name( "Suffix" );
        Map( m => m.PrimaryEmail ).Name( "Primary Email" );
        Map( m => m.DateMarkedDeceased ).Name( "Date Marked Deceased" );
        Map( m => m.HomePhoneExtension ).Name( "Home Phone Extension" );
        Map( m => m.HomePhonePrivacy ).Name( "Home Phone Privacy" );
        Map( m => m.MobilePhoneNumber ).Name( "Mobile Phone Number" );
        Map( m => m.MobilePhoneExtension ).Name( "Mobile Phone Extension" );
        Map( m => m.MobilePhonePrivacy ).Name( "Mobile Phone Privacy" );
        Map( m => m.WorkPhoneNumber ).Name( "Work Phone Number" );
        Map( m => m.WorkPhoneExtension ).Name( "Work Phone Extension" );
        Map( m => m.WorkPhonePrivacy ).Name( "Work Phone Privacy" );
        Map( m => m.OtherPhoneNumber ).Name( "Other Phone Number" );
        Map( m => m.OtherPhoneExtension ).Name( "Other Phone Extension" );
        Map( m => m.OtherPhonePrivacy ).Name( "Other Phone Privacy" );
        Map( m => m.BaptismCompleted ).Name( "Baptism Completed?" );
        Map( m => m.BaptismDate ).Name( "Baptism Date" );
        Map( m => m.VolumePageEntry ).Name( "Volume, Page, Entry" );
        Map( m => m.BaptismNotes ).Name( "Baptism Notes" );
        Map( m => m.BaptismNotesAddedBy ).Name( "Baptism Notes Added By" );
        Map( m => m.BaptismNotesLastModified ).Name( "Baptism Notes Last Modified" );
        Map( m => m.RootedCompleted ).Name( "Rooted Completed?" );
        Map( m => m.RootedDate ).Name( "Rooted Date" );
        Map( m => m.VolumePageEntry ).Name( "Volume, Page, Entry" );
        Map( m => m.RootedCompleted ).Name( "Rooted Completed?" );
        Map( m => m.RootedDate ).Name( "Rooted Date" );
        Map( m => m.RootedNotes ).Name( "Rooted Notes" );
        Map( m => m.RootedNotesAddedBy ).Name( "Rooted Notes Added By" );
        Map( m => m.RootedNotesLastModified ).Name( "Rooted Notes Last Modified" );
        Map( m => m.SalvationRededicatedLifeCompleted ).Name( "Salvation/Rededicated Life Completed?" );
        Map( m => m.SalvationRededicatedLifeDate ).Name( "Salvation/Rededicated Life Date" );
        Map( m => m.SalvationRededicatedLifeNotes ).Name( "Salvation/Rededicated Life Notes" );
        Map( m => m.SalvationRededicatedLifeNotesAddedBy ).Name( "Salvation/Rededicated Life Notes Added By" );
        Map( m => m.SalvationRededicatedLifeNotesLastModified ).Name( "Salvation/Rededicated Life Notes Last Modified" );
        Map( m => m.MarriageCompleted ).Name( "Marriage Completed?" );
        Map( m => m.MarriageDate ).Name( "Marriage Date" );
        Map( m => m.MarriageNotes ).Name( "Marriage Notes" );
        Map( m => m.MarriageNotesAddedBy ).Name( "Marriage Notes Added By" );
        Map( m => m.MarriageNotesLastModified ).Name( "Marriage Notes Last Modified" );
        Map( m => m.BibleMilestoneCompleted ).Name( "Bible Milestone Completed?" );
        Map( m => m.BibleMilestoneDate ).Name( "Bible Milestone Date" );
        Map( m => m.BibleMilestoneNotes ).Name( "Bible Milestone Notes" );
        Map( m => m.BibleMilestoneNotesAddedBy ).Name( "Bible Milestone Notes Added By" );
        Map( m => m.BibleMilestoneNotesLastModified ).Name( "Bible Milestone Notes Last Modified" );
        Map( m => m.ChildDedicationCompleted ).Name( "Child Dedication Completed?" );
        Map( m => m.ChildDedicationDate ).Name( "Child Dedication Date" );
        Map( m => m.ChildDedicationNotes ).Name( "Child Dedication Notes" );
        Map( m => m.ChildDedicationNotesAddedBy ).Name( "Child Dedication Notes Added By" );
        Map( m => m.ChildDedicationNotesLastModified ).Name( "Child Dedication Notes Last Modified" );
        Map( m => m.FamilyId ).Name( "Family Id" );
        Map( m => m.PrimaryPhoneNumber ).Name( "Primary Phone Number" );
        Map( m => m.PrimaryPhoneNumberExtension ).Name( "Primary Phone Number Extension" );
        Map( m => m.MailingAddress1 ).Name( "Mailing Address 1" );
        Map( m => m.MailingAddress2 ).Name( "Mailing Address 2" );
        Map( m => m.MailingCity ).Name( "Mailing City" );
        Map( m => m.MailingRegion ).Name( "Mailing Region" );
        Map( m => m.MailingPostalCode ).Name( "Mailing Postal Code" );
        Map( m => m.MailingCountry ).Name( "Mailing Country" );
        Map( m => m.Campus ).Name( "Campus" );
        Map( m => m.IndividualStatus ).Name( "Individual Status" );
        Map( m => m.DateOfDeath ).Name( "Date Of Death" );
        Map( m => m.MiddleName ).Name( "Middle Name" );
        Map( m => m.HomePhoneNumber ).Name( "Home Phone Number" );
        Map( m => m.GiftDatefirst ).Name( "Gift Date (first)" );
        Map( m => m.GiftDatelast ).Name( "Gift Date (last)" );
        Map( m => m.SharedGiving ).Name( "Shared Giving" );
        Map( m => m.GivingNumber ).Name( "Giving Number" );
        Map( m => m.AddedWhen ).Name( "Added When" );
        Map( m => m.AnniversaryDate ).Name( "Anniversary Date" );
        Map( m => m.AllergiesMedicalConcerns ).Name( "Allergies & Medical Concerns" );
        Map( m => m.DateofBirth ).Name( "Date of Birth" );
        Map( m => m.DateofBirthDay ).Name( "Date of Birth (Day)" );
        Map( m => m.DateofBirthMonth ).Name( "Date of Birth (Month)" );
        Map( m => m.DateofBirthYear ).Name( "Date of Birth (Year)" );
        Map( m => m.DoNotEmail ).Name( "Do Not Email" );
        Map( m => m.Gender ).Name( "Gender" );
        Map( m => m.HSGraduationYear ).Name( "HS Graduation Year" );
        Map( m => m.InsuranceCarrier ).Name( "Insurance Carrier" );
        Map( m => m.InsurancePolicyNumber ).Name( "Insurance Policy Number" );
        Map( m => m.LastBackgroundCheckDate ).Name( "Last Background Check Date" );
        Map( m => m.LastBackgroundCheckPackageName ).Name( "Last Background Check Package Name" );
        Map( m => m.LastBackgroundCheckStatus ).Name( "Last Background Check Status" );
        Map( m => m.MaidenName ).Name( "Maiden Name" );
        Map( m => m.MandatedReporterCompletion ).Name( "Mandated Reporter Completion" );
        Map( m => m.MandatedReporterExpiration ).Name( "Mandated Reporter Expiration" );
        Map( m => m.MaritalStatus ).Name( "Marital Status" );
        Map( m => m.MemberStatus ).Name( "Member Status" );
        Map( m => m.RootedGraduationDate ).Name( "Rooted Graduation Date" );
        Map( m => m.NoRegretsGoldGraduate ).Name( "No Regrets Gold Graduate" );
        Map( m => m.NoRegretsSilvergraduate ).Name( "No Regrets Silver graduate" );
    }
}
