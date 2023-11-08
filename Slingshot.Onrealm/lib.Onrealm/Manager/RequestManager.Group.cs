using System.Diagnostics;
using System.Net;
using System.Text.Json;
using HtmlAgilityPack;
using lib.Onrealm.Contracts;
using lib.Onrealm.Data;

namespace lib.Onrealm.Manager;

public partial class RequestManager
{

    public async IAsyncEnumerable<Group> GetGroupListAsync()
    {
        await InitAsync( "https://onrealm.org/FriendsChurch/Groups?showMoreOptions=true&includeInactive=true&runFilter=true" );

        int page = 0;

        while ( true )
        {
            Response<Group>? responseObj = null;
            var nvc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>( "page", page.ToString() ),
                new KeyValuePair<string, string>( "searchText", "" ),
                new KeyValuePair<string, string>( "isFiltered", "" ),

                new KeyValuePair<string, string>( "includeInactive", "true" ),
                new KeyValuePair<string, string>( "campusId", "" ),
                new KeyValuePair<string, string>( "frequency", "" ),
                new KeyValuePair<string, string>( "childCare", "" ),
                new KeyValuePair<string, string>( "time", "" ),
                new KeyValuePair<string, string>( "ageRangeType", "" ),
                new KeyValuePair<string, string>( "age", "" ),

                new KeyValuePair<string, string>( "gender", "" ),
                new KeyValuePair<string, string>( "maritalStatus", "" ),
                new KeyValuePair<string, string>( "groupTagId", "" ),
                new KeyValuePair<string, string>( "campusesEnabled", "true" ),

                new KeyValuePair<string, string>( "__RequestVerificationToken", header_ResponseVerificationToken! )
            };

            var cookieContainer = new CookieContainer();
            cookieContainer.Add( new Uri( "https://onrealm.org" ), new Cookie( "StratusWeb", cookie_StatusWeb ) );
            cookieContainer.Add( new Uri( "https://onrealm.org" ), new Cookie( "__RequestVerificationToken", cookie_RequestVerificationToken ) );

            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };

            HttpClient httpClient = new HttpClient( handler );

            try
            {
                var req = new HttpRequestMessage( HttpMethod.Post, "https://onrealm.org/FriendsChurch/Groups/GetGroupGrid" ) { Content = new FormUrlEncodedContent( nvc ) };

                var response = await httpClient.SendAsync( req );
                var data = await response.Content.ReadAsStringAsync();

                responseObj = JsonSerializer.Deserialize<Response<Group>>( data );
            }
            catch ( Exception ex )
            {
                Console.WriteLine( ex );
            }

            if ( responseObj == null || responseObj.Items.Count == 0 )
            {
                break;
            }

            page++;

            foreach ( var item in responseObj.Items )
            {
                yield return item;
            }

        }
    }

    private async Task<string?> GetGroupMinistryAreaAsync( string groupId )
    {
        try
        {

            var baseUri = new Uri( "https://onrealm.org" );

            var cookieContainer = new CookieContainer();
            cookieContainer.Add( baseUri, new Cookie( "StratusWeb", cookie_StatusWeb ) );

            var handler = new HttpClientHandler()
            {
                CookieContainer = cookieContainer,
                UseCookies = true
            };

            HttpClient httpClient = new HttpClient( handler );

            var response = await httpClient.GetAsync( $"https://onrealm.org/FriendsChurch/Groups/Group/{groupId}" );
            var content = await response.Content.ReadAsStringAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml( content );

            var inputNode = doc.DocumentNode.SelectSingleNode( "//div[@id='ministry-area']" );


            return inputNode.GetAttributeValue( "data-value", "" );
        }
        catch ( Exception ex )
        {
            Debug.Write( ex );
            throw new Exception( "", ex );
        }
    }

    public async IAsyncEnumerable<Roster> GetRosterListAsync( string groupId )
    {
        await InitAsync( $"https://onrealm.org/FriendsChurch/Groups/Rosters/{groupId}" );
        var minstryArea = await GetGroupMinistryAreaAsync( groupId );

        int page = 0;

        while ( true )
        {
            Response<Roster>? responseObj = null;
            var nvc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>( "page", page.ToString() ),
                new KeyValuePair<string, string>( "groupId", groupId ),
                new KeyValuePair<string, string>( "groupType", "Normal"),
                new KeyValuePair<string, string>( "ministryAreaId", minstryArea ?? ""),
                new KeyValuePair<string, string>( "actionName", "Rosters" ),
                new KeyValuePair<string, string>( "trackAttendance", "true" ),
                new KeyValuePair<string, string>( "searchText", "" ),
                new KeyValuePair<string, string>( "allSelected", "" ),

                new KeyValuePair<string, string>( "__RequestVerificationToken", header_ResponseVerificationToken! )
            };

            var cookieContainer = new CookieContainer();
            cookieContainer.Add( new Uri( "https://onrealm.org" ), new Cookie( "StratusWeb", cookie_StatusWeb ) );
            cookieContainer.Add( new Uri( "https://onrealm.org" ), new Cookie( "__RequestVerificationToken", cookie_RequestVerificationToken ) );

            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };

            HttpClient httpClient = new HttpClient( handler );

            try
            {
                var req = new HttpRequestMessage( HttpMethod.Post, "https://onrealm.org/FriendsChurch/Groups/GetRostersGrid" ) { Content = new FormUrlEncodedContent( nvc ) };

                var response = await httpClient.SendAsync( req );
                var data = await response.Content.ReadAsStringAsync();

                responseObj = JsonSerializer.Deserialize<Response<Roster>>( data );
            }
            catch ( Exception ex )
            {
                Debug.Write( ex );
                throw new Exception( "", ex );
            }

            if ( responseObj == null || responseObj.Items.Count == 0 )
            {
                break;
            }

            page++;

            foreach ( var item in responseObj.Items )
            {
                yield return item;
            }

        }
    }

}
