using System.Diagnostics;
using System.Net;
using HtmlAgilityPack;

namespace lib.Onrealm.Manager;

public partial class RequestManager
{
    private string cookie_StatusWeb;
    private string? cookie_RequestVerificationToken;
    private string? header_ResponseVerificationToken;

    public RequestManager( string cookie_StatusWeb )
    {

        this.cookie_StatusWeb = cookie_StatusWeb;

    }

    private async Task InitAsync( string url )
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

            var response = await httpClient.GetAsync( url );
            var content = await response.Content.ReadAsStringAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml( content );

            var inputNode = doc.DocumentNode.SelectSingleNode( "//input[@type='hidden' and @name='__RequestVerificationToken']" );

            header_ResponseVerificationToken = inputNode.GetAttributeValue( "value", "" );

            var responseCookies = cookieContainer.GetCookies( baseUri );
            cookie_RequestVerificationToken = responseCookies.Where( c => c.Name == "__RequestVerificationToken" ).FirstOrDefault()!.Value;
        }
        catch ( Exception ex )
        {
            Debug.WriteLine( ex );
            throw new Exception( "", ex );
        }

    }
}
