using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using lib.Onrealm.Data;
using lib.Onrealm.Manager;

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
    }

    private async static Task ProcessGroups( string cookie )
    {
        await GroupsData.GetAllGroups( cookie );
        var groups = GroupsData.Groups;
        var rosters = GroupsData.Rosters;
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
}
