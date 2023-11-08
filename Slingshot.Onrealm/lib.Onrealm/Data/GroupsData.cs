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
    private static ConcurrentQueue<Group> groupsQueue = new ConcurrentQueue<Group>();
    private static ConcurrentQueue<Roster> rostersQueue = new ConcurrentQueue<Roster>();

    public static List<Group> Groups { get; private set; } = new List<Group>();
    public static List<Roster> Rosters { get; private set; } = new List<Roster>();

    public static async Task GetAllGroups( string cookie )
    {
        groupsQueue.Clear();
        rostersQueue.Clear();
        Groups.Clear();
        Rosters.Clear();

        var requestManager = new RequestManager( cookie );

        Debug.WriteLine( "Loading Groups" );
        var groupList = requestManager.GetGroupListAsync();

        await foreach ( var group in groupList )
        {
            groupsQueue.Enqueue( group );
        }

        Groups.AddRange( groupsQueue );

        var series = Enumerable.Range( 1, 5 ).ToList();
        var tasks = new List<Task>();
        foreach ( var i in series )
        {
            tasks.Add( ProcessRoster( cookie ) );
        }

        await Task.WhenAll( tasks );

        Rosters.AddRange( rostersQueue );
        rostersQueue.Clear();
    }

    private static async Task ProcessRoster( string cookie )
    {
        var requestManager = new RequestManager( cookie );

        while ( true )
        {
            if ( groupsQueue.TryDequeue( out var group ) )
            {
                if ( group?.GroupId == null )
                {
                    continue;
                }

                Debug.WriteLine( $"Loading Roster For {group.GroupId}" );
                var roster = requestManager.GetRosterListAsync( group.GroupId );
                await foreach ( var individual in roster )
                {
                    rostersQueue.Enqueue( individual );
                }

            }
            else
            {
                break;
            }
        }
    }
}
