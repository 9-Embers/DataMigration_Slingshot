using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using lib.Onrealm.Contracts;
using lib.Onrealm.Data;
using lib.Onrealm.Manager;
using lib.Onrealm.Translators;
using Slingshot.Core.Utilities;
using Slingshot.Core;
using Slingshot.Core.Model;

namespace lib.Onrealm;

public static class MigrationDirector
{


    public static async Task Run( string cookie, bool clearData = false )
    {
        if ( clearData )
        {
            Database.ClearData();
        }

        await ProcessIndividuals( cookie );
        await ProcessGroups( cookie );
        await ProcessFinances( cookie );
    }

    private async static Task ProcessIndividuals( string cookie )
    {
        await IndividualData.Run( cookie );
        var individuals = IndividualData.Individuals;

        ImportPackage.InitializePackageFolder();

        WriteIndividuals( individuals, IndividualData.CampusMap, IndividualData.FamilyIdMap, IndividualData.IndividualIdMap );

        ImportPackage.FinalizePackage("realm-export.slingshot");
    }

    private async static Task ProcessGroups( string cookie )
    {
        await GroupsData.GetAllGroups( cookie );
        var groups = GroupsData.Groups;
        var rosters = GroupsData.Rosters;

        var present = rosters.Where( r => r.PresentCount.HasValue && r.PresentCount > 0 ).ToList();
        var ministryIds = rosters.Select( r => r.MinistryAreaId ).Distinct().ToList();
        var selected = rosters.Where( r => r.Selected == true ).ToList();
        var rosterTypes = rosters.Select( r => r.RosterType ).Distinct().ToList();
    }

    private async static Task ProcessFinances( string cookie )
    {
        await FinanceData.Run( cookie );

        var funds = FinanceData.Funds;
        var refunds = FinanceData.Refunds;
        var manualBatches  = FinanceData.ManualBatches;
        var onlineBatches = FinanceData.OnlineBatches;
        var manualContributions = FinanceData.ManualContributions;
        var onlineContributions = FinanceData.OnlineContributions;
    }

    private static void WriteIndividuals( List<Individual> individuals, Dictionary<string, int> campusMap, Dictionary<string, int> familyMap, Dictionary<string, int> personMap )
    {
        WritePersonAttributes();

        foreach ( var individual in individuals )
        {
            var campusId = campusMap.GetValueOrDefault( individual.Campus ) ;
            var familyId = familyMap.GetValueOrDefault( individual.FamilyId );
            var personId = personMap.GetValueOrDefault( individual.IndividualId );

            var person = OnrealmPerson.Translate( individual, personId, campusId, familyId );

            if ( person is not null )
            {
                ImportPackage.WriteToPackage( person );
            }
        }
    }

    private static void WritePersonAttributes()
    {
        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Baptism Completed",
            Key = "BaptismCompleted",
            Category = "Membership",
            FieldType = "Rock.Field.Types.BooleanFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Baptism Date",
            Key = "BaptismDate",
            Category = "Membership",
            FieldType = "Rock.Field.Types.DateFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Baptism Notes",
            Key = "BaptismNotes",
            Category = "Membership",
            FieldType = "Rock.Field.Types.TextFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Rooted Completed",
            Key = "RootedCompleted",
            Category = "Membership",
            FieldType = "Rock.Field.Types.BooleanFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Rooted Date",
            Key = "RootedDate",
            Category = "Membership",
            FieldType = "Rock.Field.Types.DateFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Rooted Notes",
            Key = "RootedNotes",
            Category = "Membership",
            FieldType = "Rock.Field.Types.TextFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Salvation/Rededicated Life Completed",
            Key = "SalvationRededicatedLifeCompleted",
            Category = "Membership",
            FieldType = "Rock.Field.Types.BooleanFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Salvation/Rededicated Life Date",
            Key = "SalvationRededicatedLifeDate",
            Category = "Membership",
            FieldType = "Rock.Field.Types.DateFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Salvation/Rededicated Life Notes",
            Key = "SalvationRededicatedLifeNotes",
            Category = "Membership",
            FieldType = "Rock.Field.Types.TextFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Bible Milestone Completed",
            Key = "BibleMilestoneCompleted",
            Category = "Membership",
            FieldType = "Rock.Field.Types.BooleanFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Bible Milestone Date",
            Key = "BibleMilestoneDate",
            Category = "Membership",
            FieldType = "Rock.Field.Types.DateFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Bible Milestone Notes",
            Key = "BibleMilestoneNotes",
            Category = "Membership",
            FieldType = "Rock.Field.Types.TextFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Marriage Completed",
            Key = "MarriageCompleted",
            Category = "Personal",
            FieldType = "Rock.Field.Types.BooleanFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Marriage Date",
            Key = "MarriageDate",
            Category = "Personal",
            FieldType = "Rock.Field.Types.DateFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Marriage Notes",
            Key = "MarriageNotes",
            Category = "Personal",
            FieldType = "Rock.Field.Types.TextFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Child Dedication Completed",
            Key = "ChildDedicationCompleted",
            Category = "Personal",
            FieldType = "Rock.Field.Types.BooleanFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Child Dedication Date",
            Key = "ChildDedicationDate",
            Category = "Personal",
            FieldType = "Rock.Field.Types.DateFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Child Dedication Notes",
            Key = "ChildDedicationNotes",
            Category = "Personal",
            FieldType = "Rock.Field.Types.TextFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Gift Date First",
            Key = "GiftDateFirst",
            Category = "Giving",
            FieldType = "Rock.Field.Types.DateFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Gift Date Last",
            Key = "GiftDateLast",
            Category = "Giving",
            FieldType = "Rock.Field.Types.DateFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Shared Giving",
            Key = "SharedGiving",
            Category = "Giving",
            FieldType = "Rock.Field.Types.BooleanFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Giving Number",
            Key = "GivingNumber",
            Category = "Giving",
            FieldType = "Rock.Field.Types.TextFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Allergies/Medical Concerns",
            Key = "AllergiesMedicalConcerns",
            Category = "Personal",
            FieldType = "Rock.Field.Types.TextFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "HS Graduation Year",
            Key = "HSGraduationYear",
            Category = "Personal",
            FieldType = "Rock.Field.Types.TextFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Insurance Carrier",
            Key = "InsuranceCarrier",
            Category = "Legal",
            FieldType = "Rock.Field.Types.TextFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Insurance Policy Number",
            Key = "InsurancePolicyNumber",
            Category = "Legal",
            FieldType = "Rock.Field.Types.TextFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Last Background Check Date",
            Key = "LastBackgroundCheckDate",
            Category = "Legal",
            FieldType = "Rock.Field.Types.DateFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Last Background Check Package Name",
            Key = "LastBackgroundCheckPackageName",
            Category = "Legal",
            FieldType = "Rock.Field.Types.TextFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Last Background Check Status",
            Key = "LastBackgroundCheckStatus",
            Category = "Legal",
            FieldType = "Rock.Field.Types.TextFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Maiden Name",
            Key = "MaidenName",
            Category = "Personal",
            FieldType = "Rock.Field.Types.TextFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Mandated Reporter Completion",
            Key = "MandatedReporterCompletion",
            Category = "Legal",
            FieldType = "Rock.Field.Types.BooleanFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Mandated Reporter Expiration",
            Key = "MandatedReporterExpiration",
            Category = "Legal",
            FieldType = "Rock.Field.Types.DateFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "Rooted Graduation Date",
            Key = "RootedGraduationDate",
            Category = "Membership",
            FieldType = "Rock.Field.Types.DateFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "No Regrets Gold Graduate",
            Key = "NoRegretsGoldGraduate",
            Category = "Personal",
            FieldType = "Rock.Field.Types.TextFieldType",
        });

        ImportPackage.WriteToPackage(new PersonAttribute()
        {
            Name = "No Regrets Silver Graduate",
            Key = "NoRegretsSilverGraduate",
            Category = "Personal",
            FieldType = "Rock.Field.Types.TextFieldType",
        });

    }
}
