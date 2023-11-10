using System.Diagnostics;
using lib.Onrealm.Contracts;
using lib.Onrealm.Manager;

namespace lib.Onrealm.Data;

public static class IndividualData
{
    public static List<Individual> Individuals { get; set; } = new List<Individual>();

    public static async Task Run( string cookie )
    {
        Debug.WriteLine( "Loading Individuals" );
        var requestManager = new RequestManager( cookie );
        Individuals = await requestManager.GetIndividualListAsync();
    }
}