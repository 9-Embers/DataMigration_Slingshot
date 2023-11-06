using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HtmlAgilityPack;
using lib.Onrealm.Contracts;

namespace lib.Onrealm.Manager;

public partial class RequestManager
{
    public async IAsyncEnumerable<Individual> GetIndividualListAsync()
    {
        await InitAsync( "https://onrealm.org/FriendsChurch/Home/Tasks?redirectController=Individual&redirectAction=Info&redirectId=a0ef25dd-0dbd-4968-9f21-b0990135208f" );

        int page = 0;

        while ( true )
        {
            Response<Individual>? responseObj = null;
            var nvc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>( "page", page.ToString() ),
                new KeyValuePair<string, string>( "q", "" ),
                new KeyValuePair<string, string>( "type", "All" ),
                new KeyValuePair<string, string>( "status", "All" ),
                new KeyValuePair<string, string>( "personnelStatus", "0" ),
                new KeyValuePair<string, string>( "__RequestVerificationToken", header_ResponseVerificationToken! )
            };

            var cookieContainer = new CookieContainer();
            cookieContainer.Add( new Uri( "https://onrealm.org" ), new Cookie( "StratusWeb", cookie_StatusWeb ) );
            cookieContainer.Add( new Uri( "https://onrealm.org" ), new Cookie( "__RequestVerificationToken", cookie_RequestVerificationToken ) );

            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };

            HttpClient httpClient = new HttpClient( handler );

            try
            {
                var req = new HttpRequestMessage( HttpMethod.Post, "https://onrealm.org/FriendsChurch/Individual/GetIndividualGrid" ) { Content = new FormUrlEncodedContent( nvc ) };

                var response = await httpClient.SendAsync( req );
                var data = await response.Content.ReadAsStringAsync();

                responseObj = JsonSerializer.Deserialize<Response<Individual>>( data );
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
}
