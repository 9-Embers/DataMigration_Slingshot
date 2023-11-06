using System.Diagnostics;
using System.Net;
using System.Text.Json;
using HtmlAgilityPack;
using lib.Onrealm.Contracts;

namespace lib.Onrealm.Manager;

public partial class RequestManager
{

    public async IAsyncEnumerable<Fund> GetFundsListAsync( string showactive = "true" )
    {
        await InitAsync( "https://onrealm.org/FriendsChurch/Funds" );

        int page = 0;

        while ( true )
        {
            Response<Fund>? responseObj = null;
            var nvc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>( "page", page.ToString() ),
                new KeyValuePair<string, string>( "showActive", showactive ),
                new KeyValuePair<string, string>( "IsGivingIntegration", "" ),
                new KeyValuePair<string, string>( "isFiltered", "" ),

                new KeyValuePair<string, string>( "__RequestVerificationToken", header_ResponseVerificationToken! )
            };

            var cookieContainer = new CookieContainer();
            cookieContainer.Add( new Uri( "https://onrealm.org" ), new Cookie( "StratusWeb", cookie_StatusWeb ) );
            cookieContainer.Add( new Uri( "https://onrealm.org" ), new Cookie( "__RequestVerificationToken", cookie_RequestVerificationToken ) );

            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };

            HttpClient httpClient = new HttpClient( handler );

            try
            {
                var req = new HttpRequestMessage( HttpMethod.Post, "https://onrealm.org/FriendsChurch/Funds/GetFundsGrid" ) { Content = new FormUrlEncodedContent( nvc ) };

                var response = await httpClient.SendAsync( req );
                var data = await response.Content.ReadAsStringAsync();

                responseObj = JsonSerializer.Deserialize<Response<Fund>>( data );
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

            //Recursively do a second pass for inactive accounts
            if ( showactive == "true" )
            {
                var inactive = GetFundsListAsync( "false" );
                await foreach ( var item in inactive )
                {
                    yield return item;
                }
            }

        }
    }

    public async IAsyncEnumerable<Batch> GetManualBatchListAsync()
    {
        await InitAsync( "https://onrealm.org/FriendsChurch/Giving/Batches?isPosted=True" );

        int page = 0;

        while ( true )
        {
            Response<Batch>? responseObj = null;
            var nvc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>( "page", page.ToString() ),
                new KeyValuePair<string, string>( "sortName", "Date" ),
                new KeyValuePair<string, string>( "sortAscending", "false" ),
                new KeyValuePair<string, string>( "batchNumberFilterFrom", "" ),
                new KeyValuePair<string, string>( "batchNumberFilterTo", "" ),
                new KeyValuePair<string, string>( "giftAmountFilter", "" ),
                new KeyValuePair<string, string>( "itemDateFilter", "{\"DateRangeType\":\"Custom\",\"BeginDate\":\"11/06/2010\",\"EndDate\":\"12/31/2030\"}" ),
                new KeyValuePair<string, string>( "depositAccountsFilter", "" ),

                new KeyValuePair<string, string>( "__RequestVerificationToken", header_ResponseVerificationToken! )
            };

            var cookieContainer = new CookieContainer();
            cookieContainer.Add( new Uri( "https://onrealm.org" ), new Cookie( "StratusWeb", cookie_StatusWeb ) );
            cookieContainer.Add( new Uri( "https://onrealm.org" ), new Cookie( "__RequestVerificationToken", cookie_RequestVerificationToken ) );

            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };

            HttpClient httpClient = new HttpClient( handler );

            try
            {
                var req = new HttpRequestMessage( HttpMethod.Post, "https://onrealm.org/FriendsChurch/Giving/GetBatchesGrid" ) { Content = new FormUrlEncodedContent( nvc ) };

                var response = await httpClient.SendAsync( req );
                var data = await response.Content.ReadAsStringAsync();

                responseObj = JsonSerializer.Deserialize<Response<Batch>>( data );
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

    public async Task<List<ManualContribution>> GetManualBatchContributionsAsync( string batchId )
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

            var response = await httpClient.GetAsync( $"https://onrealm.org/FriendsChurch/Giving/Gifts/{batchId}" );
            var content = await response.Content.ReadAsStringAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml( content );

            var rows = doc.DocumentNode.SelectNodes( "//tr" );

            var datas = new List<ManualContribution>();

            foreach ( var row in rows )
            {


                var cells = row.Descendants( "td" ).ToList();

                if ( cells.Count < 6 )
                {
                    continue;
                }


                var individualNode = cells[1].Descendants( "a" ).FirstOrDefault();
                if ( individualNode == null )
                {
                    continue;
                }

                var personLink = individualNode.GetAttributeValue( "href", "" );

                var contribution = new ManualContribution
                {
                    PersonId = personLink.Substring( personLink.Length - 36 ),
                    PaymentForm = GetText( cells[2] ),
                    GiftType = GetText( cells[3] ),
                    CheckNumber = GetText( cells[4] ),
                    Fund = GetText( cells[5] ),
                    Amount = GetText( cells[6] )
                };

                datas.Add( contribution );
            }

            return datas;
        }
        catch ( Exception ex )
        {
            Debug.Write( ex );
            throw new Exception( "", ex );
        }
    }


    public async IAsyncEnumerable<Batch> GetOnlineBatchListAsync()
    {
        await InitAsync( "https://onrealm.org/FriendsChurch/Giving/OnlineBatches?page=0&itemDateFilter={%22DateRangeType%22:%22Custom%22,%22BeginDate%22:%2211/06/2010%22,%22EndDate%22:%2212/31/2030%22}" );

        int page = 0;

        while ( true )
        {
            Response<Batch>? responseObj = null;
            var nvc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>( "page", page.ToString() ),
                new KeyValuePair<string, string>( "sortName", "BatchDate" ),
                new KeyValuePair<string, string>( "sortAscending", "false" ),
                new KeyValuePair<string, string>( "batchNumberFilterFrom", "" ),
                new KeyValuePair<string, string>( "batchNumberFilterTo", "" ),
                new KeyValuePair<string, string>( "giftAmountFilter", "" ),
                new KeyValuePair<string, string>( "itemDateFilter", "{\"DateRangeType\":\"Custom\",\"BeginDate\":\"11/06/2010\",\"EndDate\":\"12/31/2030\"}" ),
                new KeyValuePair<string, string>( "depositAccountsFilter", "" ),

                new KeyValuePair<string, string>( "__RequestVerificationToken", header_ResponseVerificationToken! )
            };

            var cookieContainer = new CookieContainer();
            cookieContainer.Add( new Uri( "https://onrealm.org" ), new Cookie( "StratusWeb", cookie_StatusWeb ) );
            cookieContainer.Add( new Uri( "https://onrealm.org" ), new Cookie( "__RequestVerificationToken", cookie_RequestVerificationToken ) );

            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };

            HttpClient httpClient = new HttpClient( handler );

            try
            {
                var req = new HttpRequestMessage( HttpMethod.Post, "https://onrealm.org/FriendsChurch/Giving/GetOnlineBatchesGrid" ) { Content = new FormUrlEncodedContent( nvc ) };

                var response = await httpClient.SendAsync( req );
                var data = await response.Content.ReadAsStringAsync();

                responseObj = JsonSerializer.Deserialize<Response<Batch>>( data );
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

    public async Task<List<OnlineContribution>> GetOnlineBatchContributionsAsync( string batchId )
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

            var response = await httpClient.GetAsync( $"https://onrealm.org/FriendsChurch/Giving/Gifts/{batchId}" );
            var content = await response.Content.ReadAsStringAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml( content );

            var rows = doc.DocumentNode.SelectNodes( "//tr" );

            var datas = new List<OnlineContribution>();

            foreach ( var row in rows )
            {


                var cells = row.Descendants( "td" ).ToList();

                if ( cells.Count < 6 )
                {
                    continue;
                }


                var individualNode = cells[0].Descendants( "a" ).FirstOrDefault();
                if ( individualNode == null )
                {
                    continue;
                }

                var personLink = individualNode.GetAttributeValue( "href", "" );

                var contribution = new OnlineContribution
                {
                    PersonId = personLink.Substring( personLink.Length - 36 ),
                    GiftType = GetText( cells[1] ),
                    Account = GetText( cells[2] ),
                    GiftDate = GetText( cells[3] ),
                    DepositDate = GetText( cells[4] ),
                    ReceiptNumber = GetText( cells[5] ),
                    Fund = GetText( cells[6] ),
                    Amount = GetText( cells[7] )
                };

                datas.Add( contribution );
            }

            return datas;
        }
        catch ( Exception ex )
        {
            Debug.Write( ex );
            throw new Exception( "", ex );
        }
    }

    public async Task<List<Refund>> GetRefundsAsync()
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

            var response = await httpClient.GetAsync( $"https://onrealm.org/FriendsChurch/Giving/RefundedGifts" );
            var content = await response.Content.ReadAsStringAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml( content );

            var rows = doc.DocumentNode.SelectNodes( "//tr" );

            var datas = new List<Refund>();

            foreach ( var row in rows )
            {


                var cells = row.Descendants( "td" ).ToList();

                if ( cells.Count < 6 )
                {
                    continue;
                }


                var individualNode = cells[2].Descendants( "a" ).FirstOrDefault();
                if ( individualNode == null )
                {
                    continue;
                }

                var personLink = individualNode.GetAttributeValue( "href", "" );

                var refund = new Refund
                {
                    PersonId = personLink.Substring( personLink.Length - 36 ),
                    RefundedDate = GetText( cells[0] ),
                    BatchNumber = GetText( cells[1] ),
                    Account = GetText( cells[3] ),
                    Fund = GetText( cells[4] ),
                    RefundedAmount = GetText( cells[5] ),
                };

                datas.Add( refund );
            }

            return datas;
        }
        catch ( Exception ex )
        {
            Debug.Write( ex );
            throw new Exception( "", ex );
        }
    }


    private string GetText( HtmlNode? value )
    {
        if ( value == null )
        {
            return "";
        }

        return string.Join( " ", value.Descendants().Where( c => c.NodeType == HtmlNodeType.Text ).Select( x => x.InnerText.Trim() ) ).Trim();
        ;
    }
}
