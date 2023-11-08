using System.Reflection;
using System.Text.Json;
using lib.Onrealm.Data;

namespace lib.Onrealm;

public static class MigrationDirector
{

    public static bool StartFromFiles { get; set; } = false;

    public static async Task Run( string cookie, bool startFromFiles = false )
    {
        StartFromFiles = startFromFiles;
        await ProcessGroups( cookie );
        //await ProcessFinances( cookie );
    }


    private async static Task ProcessGroups( string cookie )
    {
        if ( !StartFromFiles )
        {
            await GroupsData.GetAllGroups( cookie );
            SaveObject( GroupsData.Groups, "groups.json" );
            SaveObject( GroupsData.Rosters, "rosters.json" );
        }
    }

    private async static Task ProcessFinances( string cookie )
    {
        if ( !StartFromFiles )
        {
            await FinanceData.Run( cookie );
            SaveObject( FinanceData.Funds, "funds.json" );
            SaveObject( FinanceData.ManualBatches, "manualbatches.json" );
            SaveObject( FinanceData.OnlineBatches, "onlinebatches.json" );
            SaveObject( FinanceData.ManualContributions, "manualcontributions.json" );
            SaveObject( FinanceData.OnlineContributions, "onlinecontributions.json" );
            SaveObject( FinanceData.Refunds, "refunds.json" );
        }
    }

    private static T LoadObject<T>( string filename )
    {
        var json = File.ReadAllText( Path.Combine( System.IO.Path.GetDirectoryName( Assembly.GetEntryAssembly()!.Location )!, filename ) );
        return JsonSerializer.Deserialize<T>( json )!;
    }

    private static void SaveObject( object obj, string filename )
    {
        try
        {

            var json = JsonSerializer.Serialize( obj );
            File.WriteAllText( Path.Combine( System.IO.Path.GetDirectoryName( Assembly.GetEntryAssembly()!.Location )!, filename ), json );

        }
        catch ( Exception ex )
        {

        }
    }
}
