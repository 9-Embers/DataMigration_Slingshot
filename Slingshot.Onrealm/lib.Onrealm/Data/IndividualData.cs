using System.Diagnostics;
using lib.Onrealm.Contracts;
using lib.Onrealm.Manager;
using Slingshot.Core;

namespace lib.Onrealm.Data;

public static class IndividualData
{
    public static List<Individual> Individuals { get; set; } = new List<Individual>();

    public static Dictionary<string, int> IndividualIdMap { get; set; } = new Dictionary<string, int>();

    public static Dictionary<string, int> FamilyIdMap { get; set; } = new Dictionary<string, int>();

    public static Dictionary<string, int> CampusMap { get; set; } = new Dictionary<string, int>();

    public static async Task Run( string cookie )
    {
        Debug.WriteLine( "Loading Individuals" );
        var requestManager = new RequestManager( cookie );
        Individuals = await requestManager.GetIndividualListAsync();
        
        foreach ( var individual in Individuals )
        {
            if ( individual.IndividualId is not null 
                && individual.IndividualId.IsNotNullOrWhitespace() 
                && IndividualIdMap.ContainsKey( individual.IndividualId ) == false )
            {
                IndividualIdMap.Add( individual.IndividualId, IndividualIdMap.Count + 1 );
            }

            if ( individual.FamilyId is not null 
                               && individual.FamilyId.IsNotNullOrWhitespace() 
                                              && FamilyIdMap.ContainsKey( individual.FamilyId ) == false )
            {
                FamilyIdMap.Add( individual.FamilyId, FamilyIdMap.Count + 1 );
            }

            if ( individual.Campus is not null 
                               && individual.Campus.IsNotNullOrWhitespace() 
                                              && CampusMap.ContainsKey( individual.Campus ) == false )
            {
                CampusMap.Add( individual.Campus, CampusMap.Count + 1 );
            }
        }
    }
}