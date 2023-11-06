using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib.Onrealm.Contracts;
using lib.Onrealm.Manager;

namespace lib.Onrealm.Data;

public static class GroupsData
{
    private static ConcurrentQueue<Group> groups = new ConcurrentQueue<Group>();
    private static ConcurrentQueue<Roster> rosters = new ConcurrentQueue<Roster>();

    public static async Task GetAllGroups( string cookie )
    {
        groups.Clear();

        var requestManager = new RequestManager( cookie );

        var groupList = requestManager.GetGroupListAsync();

        await foreach ( var group in groupList )
        {
            groups.Enqueue( group );
        }

        var series = Enumerable.Range( 1, 3 ).ToList();
        var tasks = new List<Task>();
        foreach ( var i in series )
        {
            tasks.Add( ProcessRoster( cookie ) );
        }

        await Task.WhenAll( tasks );

        foreach ( var roster in rosters )
        {
            Debug.WriteLine( roster.IndividualId + ":" + roster.GroupId );
        }
    }

    public static async Task ProcessRoster( string cookie )
    {
        Debug.WriteLine( "ProcessRosterStarted" );
        var requestManager = new RequestManager( cookie );

        while ( true )
        {
            if (groups.TryDequeue( out var group ) )
            {
                if (group?.GroupId == null )
                {
                    continue;
                }

                var roster =  requestManager.GetRosterListAsync( group.GroupId );
                await foreach (var individual in roster )
                {
                    Debug.WriteLine( individual.RosterId );
                    rosters.Enqueue( individual );
                }

            }
            else
            {
                break;
            }
        }
    }
}
