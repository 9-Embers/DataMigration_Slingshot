using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using lib.Onrealm.Contracts;
using lib.Onrealm.Manager;
using lib.Onrealm.Model;

namespace lib.Onrealm.Data;

public static class GroupsData
{
    private static ConcurrentQueue<Group> groupsQueue = new ConcurrentQueue<Group>();

    public static List<Group> Groups { get; private set; } = new List<Group>();
    public static List<Roster> Rosters { get; private set; } = new List<Roster>();

    public static async Task GetAllGroups( string cookie )
    {
        Database.ExecuteNonQuery( "CREATE TABLE IF NOT EXISTS [Group] (Id INTEGER PRIMARY KEY AUTOINCREMENT, GroupId TEXT, GroupJson TEXT, GroupRosterJson TEXT)" );

        groupsQueue.Clear();
        Groups.Clear();
        Rosters.Clear();

        var requestManager = new RequestManager( cookie );


        if ( !Database.ExecuteQuery<int>( "SELECT Id FROM [Group]" ).Any() )
        {
            try
            {

                await LoadGroups( requestManager );

            }
            catch ( Exception ex )
            {
                Database.ExecuteNonQuery( "DROP TABLE [Group]" );
                Debug.WriteLine( ex );
                throw new Exception( "", ex );
            }
        }

        var qry = Database.ExecuteQuery<SerializedGroup>( "SELECT * FROM [Group] WHERE GroupRosterJson is null" );
        foreach ( var item in qry )
        {
            groupsQueue.Enqueue( JsonSerializer.Deserialize<Group>( item.GroupJson! )! );
        }

        var series = Enumerable.Range( 1, 5 ).ToList();
        var tasks = new List<Task>();
        foreach ( var i in series )
        {
            tasks.Add( ProcessRoster( cookie ) );
        }

        await Task.WhenAll( tasks );

        var groupsQry = Database.ExecuteQuery<SerializedGroup>( "SELECT * FROM [Group]" );
        foreach ( var item in groupsQry )
        {
            Groups.Add( JsonSerializer.Deserialize<Group>( item.GroupJson! )! );
            Rosters.AddRange( JsonSerializer.Deserialize<List<Roster>>( item.GroupRosterJson! )! );
        }
    }

    private static async Task LoadGroups( RequestManager requestManager )
    {
        Debug.WriteLine( "Loading Groups" );
        var groupList = requestManager.GetGroupListAsync();


        await foreach ( var group in groupList )
        {
            Database.ExecuteNonQuery( "INSERT INTO [Group] (GroupId,GroupJson) Values ($groupId,$groupJson)",
                new Dictionary<string, string>
                {
                    { "$groupId", group.GroupId! },
                    { "$groupJson", JsonSerializer.Serialize(group) }
                } );
        }


    }

    private static async Task ProcessRoster( string cookie )
    {
        var requestManager = new RequestManager( cookie );

        while ( true )
        {
            try
            {

                if ( groupsQueue.TryDequeue( out var group ) )
                {
                    var rosterList = new List<Roster>();

                    if ( group?.GroupId == null )
                    {
                        continue;
                    }

                    Debug.WriteLine( $"Loading Roster For {group.GroupId}" );
                    var rosters = requestManager.GetRosterListAsync( group.GroupId );
                    await foreach ( var roster in rosters )
                    {
                        rosterList.Add( roster );
                    }
                    Database.ExecuteNonQuery( "UPDATE [Group] SET GroupRosterJson = $groupRosterJson WHERE GroupId = $groupId",
                                            new Dictionary<string, string> {
                                            { "$groupRosterJson", JsonSerializer.Serialize(rosterList) },
                                            { "$groupId", group.GroupId}
                                            } );
                }
                else
                {
                    break;
                }
            }
            catch ( Exception ex )
            {
                Debug.WriteLine( ex );
                throw new Exception( "", ex );
            }
        }
    }
}
