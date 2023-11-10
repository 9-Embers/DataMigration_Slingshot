﻿using System.Globalization;
using System.Net;
using CsvHelper;
using CsvHelper.Configuration;
using lib.Onrealm.Contracts;

namespace lib.Onrealm.Manager;

public partial class RequestManager
{
    public async IAsyncEnumerable<Individual> GetIndividualListAsync()
    {
        await InitAsync( "https://onrealm.org/FriendsChurch/Queries" );


        Response<Individual>? responseObj = null;
        var nvc = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>( "query-id","" ),
                new KeyValuePair<string, string>( "query-type", "" ),
                new KeyValuePair<string, string>( "query-args", "{\"Search\":0,\"DisplayColumns\":[\"Campus0\",\"IndividualStatus\",\"DateDeceased.DateDeceased\",\"MiddleName\",\"PhoneNumber\",\"FirstGiftDate\",\"LastGiftDate\",\"SharedGiving\",\"GivingNumber\",\"Sacramentdabe75ab420348d994a4a83800f73705\",\"Sacramentaf558ad7b5554cbda071a83800f7370a\",\"Sacramente7e76437991a4cbdb0a0a83800f7370f\",\"Sacramenta2aa73605c4a42d38753a83800f7370f\",\"Sacrament0b5320cca5154d929b70a83800f73713\",\"Sacrament0f9d5ddab684481ba216a83800f73713\",\"AddedWhen\",\"Profilee8f9502e0f304004b30ea82301001bb8\",\"SystemAllergies\",\"Birthday\",\"BirthdayDay\",\"BirthdayMonth\",\"BirthdayYear\",\"Profile5972f7a91ca640e8ba28ae2a016dfabe\",\"SystemGender\",\"Profileb4bfccedfdaa4f02a466a95701014136\",\"Profile1a419a7e62c04bf9a7c5acf3014e0cf3\",\"Profile596cbbc385934da58ce4acf3014e42de\",\"BackgroundCheckDate\",\"BackgroundCheckPackageName\",\"BackgroundCheckStatus\",\"Profilec32cdb0338484ca1b254af5d01359b20\",\"Profileb2db6ff2c2514f8f8815ae540144a808\",\"Profileadefa2261f1243b8b450ae540144bc0d\",\"SystemMaritalStatus\",\"SystemStatus\",\"Profile51529dc0217d495891fdacca0173dcf3\",\"Profile3810d9ab461a40f2b3b0ae51000d8d75\",\"Profile23e1a5b107a84bf6a16eae51000d5294\"],\"Query\":[[{\"Message\":\"\",\"SubQuery\":[],\"Name\":\"Campus\",\"Operator\":3,\"Values\":[\"\"],\"Unit\":null,\"IgnoreYear\":false,\"Id\":\"Campus0\"}]],\"ResultType\":9,\"DisplayContactInfo\":1,\"IncludeDeactivated\":true,\"IncludeDeceased\":true,\"OrderBy\":null,\"Included\":[],\"Excluded\":[],\"IsReport\":false,\"PersonnelStatuses\":null,\"LabelTypes\":[\"DisplayName\"]}" ),
                new KeyValuePair<string, string>( "query-name", "" ),
                new KeyValuePair<string, string>( "query-description", "" ),
                new KeyValuePair<string, string>( "query-initial-data", "" ),
                new KeyValuePair<string, string>( "query-columns", "[{\"Id\":\"\",\"Name\":\"IndividualId\",\"Header\":\"Individual+Id\",\"Selectable\":false,\"Selected\":false,\"Json\":false,\"DataType\":\"Guid\",\"QueryDataType\":null},{\"Id\":\"ContactInfo_Label\",\"Name\":\"ContactInfo_Label\",\"Header\":\"Label\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":null},{\"Id\":\"ContactInfo_Title\",\"Name\":\"ContactInfo_Title\",\"Header\":\"Title\",\"Selectable\":true,\"Selected\":false,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":null},{\"Id\":\"ContactInfo_FirstName\",\"Name\":\"ContactInfo_FirstName\",\"Header\":\"First+Name\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":null},{\"Id\":\"ContactInfo_PreferredName\",\"Name\":\"ContactInfo_PreferredName\",\"Header\":\"Preferred+Name\",\"Selectable\":true,\"Selected\":false,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":null},{\"Id\":\"ContactInfo_LastName\",\"Name\":\"ContactInfo_LastName\",\"Header\":\"Last+Name\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":null},{\"Id\":\"ContactInfo_Suffix\",\"Name\":\"ContactInfo_Suffix\",\"Header\":\"Suffix\",\"Selectable\":true,\"Selected\":false,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":null},{\"Id\":\"ContactInfo_PrimaryEmail\",\"Name\":\"ContactInfo_PrimaryEmail\",\"Header\":\"Primary+Email\",\"Selectable\":true,\"Selected\":false,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":null},{\"Id\":\"ContactInfo_DateMarkedDeceased\",\"Name\":\"ContactInfo_DateMarkedDeceased\",\"Header\":\"Date+Marked+Deceased\",\"Selectable\":true,\"Selected\":false,\"Json\":false,\"DataType\":\"DateTime\",\"QueryDataType\":null},{\"Id\":\"ContactInfo_IndividualType\",\"Name\":\"ContactInfo_IndividualType\",\"Header\":\"Individual+Type\",\"Selectable\":true,\"Selected\":false,\"Json\":false,\"DataType\":\"Int32\",\"QueryDataType\":null},{\"Id\":\"ContactInfo_IsIndividual\",\"Name\":\"ContactInfo_IsIndividual\",\"Header\":\"Is+Individual\",\"Selectable\":false,\"Selected\":false,\"Json\":false,\"DataType\":\"Boolean\",\"QueryDataType\":null},{\"Id\":\"ContactInfo_PersonnelStatus\",\"Name\":\"ContactInfo_PersonnelStatus\",\"Header\":\"Personnel+Status\",\"Selectable\":false,\"Selected\":false,\"Json\":false,\"DataType\":\"Int16\",\"QueryDataType\":null},{\"Id\":\"ContactInfo_FamilyId\",\"Name\":\"ContactInfo_FamilyId\",\"Header\":\"Family+Id\",\"Selectable\":false,\"Selected\":false,\"Json\":false,\"DataType\":\"Guid\",\"QueryDataType\":null},{\"Id\":\"ContactInfo_PrimaryPhoneNumber\",\"Name\":\"ContactInfo_PrimaryPhoneNumber\",\"Header\":\"Primary+Phone+Number\",\"Selectable\":true,\"Selected\":false,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":null},{\"Id\":\"ContactInfo_PrimaryPhoneNumberExtension\",\"Name\":\"ContactInfo_PrimaryPhoneNumberExtension\",\"Header\":\"Primary+Phone+Number+Extension\",\"Selectable\":true,\"Selected\":false,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":null},{\"Id\":\"ContactInfo_MailingAddress1\",\"Name\":\"ContactInfo_MailingAddress1\",\"Header\":\"Mailing+Address+1\",\"Selectable\":true,\"Selected\":false,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":null},{\"Id\":\"ContactInfo_MailingAddress2\",\"Name\":\"ContactInfo_MailingAddress2\",\"Header\":\"Mailing+Address+2\",\"Selectable\":true,\"Selected\":false,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":null},{\"Id\":\"ContactInfo_MailingCity\",\"Name\":\"ContactInfo_MailingCity\",\"Header\":\"Mailing+City\",\"Selectable\":true,\"Selected\":false,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":null},{\"Id\":\"ContactInfo_MailingRegion\",\"Name\":\"ContactInfo_MailingRegion\",\"Header\":\"Mailing+Region\",\"Selectable\":true,\"Selected\":false,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":null},{\"Id\":\"ContactInfo_MailingPostalCode\",\"Name\":\"ContactInfo_MailingPostalCode\",\"Header\":\"Mailing+Postal+Code\",\"Selectable\":true,\"Selected\":false,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":null},{\"Id\":\"ContactInfo_MailingCountry\",\"Name\":\"ContactInfo_MailingCountry\",\"Header\":\"Mailing+Country\",\"Selectable\":true,\"Selected\":false,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":null},{\"Id\":null,\"Name\":\"ContactInfo_ChurchPosition\",\"Header\":null,\"Selectable\":false,\"Selected\":false,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":null},{\"Id\":null,\"Name\":\"ContactInfo_AssignmentLabel\",\"Header\":null,\"Selectable\":false,\"Selected\":false,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":null},{\"Id\":null,\"Name\":\"ContactInfo_AssignmentLocation\",\"Header\":null,\"Selectable\":false,\"Selected\":false,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":null},{\"Id\":\"Campus0\",\"Name\":\"Campus_Campus0\",\"Header\":\"Campus\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":6},{\"Id\":\"\",\"Name\":\"IndividualStatus\",\"Header\":\"Individual+Status\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":null},{\"Id\":\"DateDeceased.DateDeceased\",\"Name\":\"DateDeceased-DateDeceased\",\"Header\":\"Date+Of+Death\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":1},{\"Id\":\"\",\"Name\":\"MiddleName\",\"Header\":\"Middle+Name\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":0},{\"Id\":\"\",\"Name\":\"PhoneNumber\",\"Header\":\"Phone+Number\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":0},{\"Id\":\"\",\"Name\":\"FirstGiftDate\",\"Header\":\"Gift+Date+(first)\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"DateTime\",\"QueryDataType\":1},{\"Id\":\"\",\"Name\":\"LastGiftDate\",\"Header\":\"Gift+Date+(last)\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"DateTime\",\"QueryDataType\":1},{\"Id\":\"\",\"Name\":\"SharedGiving\",\"Header\":\"Shared+Giving\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"Boolean\",\"QueryDataType\":4},{\"Id\":\"\",\"Name\":\"GivingNumber\",\"Header\":\"Giving+Number\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"Int32\",\"QueryDataType\":2},{\"Id\":\"\",\"Name\":\"Sacrament.Sacramentdabe75ab420348d994a4a83800f73705\",\"Header\":\"Baptism+Completed?\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":4},{\"Id\":\"SubSacramentDateSacramentdabe75ab420348d994a4a83800f73705\",\"Name\":\"Sacrament.SubSacramentDateSacramentdabe75ab420348d994a4a83800f73705\",\"Header\":\"Baptism+Date\",\"Selectable\":false,\"Selected\":false,\"Json\":false,\"DataType\":\"string\",\"QueryDataType\":null},{\"Id\":\"SacramentVPESacramentdabe75ab420348d994a4a83800f73705\",\"Name\":\"Sacrament.SubSacramentVPESacramentdabe75ab420348d994a4a83800f73705\",\"Header\":\"Volume,+Page,+Entry\",\"Selectable\":false,\"Selected\":false,\"Json\":false,\"DataType\":\"string\",\"QueryDataType\":null},{\"Id\":\"\",\"Name\":\"Sacrament.Sacramentaf558ad7b5554cbda071a83800f7370a\",\"Header\":\"Rooted+Completed?\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":4},{\"Id\":\"SubSacramentDateSacramentaf558ad7b5554cbda071a83800f7370a\",\"Name\":\"Sacrament.SubSacramentDateSacramentaf558ad7b5554cbda071a83800f7370a\",\"Header\":\"Rooted+Date\",\"Selectable\":false,\"Selected\":false,\"Json\":false,\"DataType\":\"string\",\"QueryDataType\":null},{\"Id\":\"SacramentVPESacramentaf558ad7b5554cbda071a83800f7370a\",\"Name\":\"Sacrament.SubSacramentVPESacramentaf558ad7b5554cbda071a83800f7370a\",\"Header\":\"Volume,+Page,+Entry\",\"Selectable\":false,\"Selected\":false,\"Json\":false,\"DataType\":\"string\",\"QueryDataType\":null},{\"Id\":\"\",\"Name\":\"Sacrament.Sacramente7e76437991a4cbdb0a0a83800f7370f\",\"Header\":\"Salvation/Rededicated+Life+Completed?\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":4},{\"Id\":\"SubSacramentDateSacramente7e76437991a4cbdb0a0a83800f7370f\",\"Name\":\"Sacrament.SubSacramentDateSacramente7e76437991a4cbdb0a0a83800f7370f\",\"Header\":\"Salvation/Rededicated+Life+Date\",\"Selectable\":false,\"Selected\":false,\"Json\":false,\"DataType\":\"string\",\"QueryDataType\":null},{\"Id\":\"SacramentVPESacramente7e76437991a4cbdb0a0a83800f7370f\",\"Name\":\"Sacrament.SubSacramentVPESacramente7e76437991a4cbdb0a0a83800f7370f\",\"Header\":\"Volume,+Page,+Entry\",\"Selectable\":false,\"Selected\":false,\"Json\":false,\"DataType\":\"string\",\"QueryDataType\":null},{\"Id\":\"\",\"Name\":\"Sacrament.Sacramenta2aa73605c4a42d38753a83800f7370f\",\"Header\":\"Marriage+Completed?\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":4},{\"Id\":\"SubSacramentDateSacramenta2aa73605c4a42d38753a83800f7370f\",\"Name\":\"Sacrament.SubSacramentDateSacramenta2aa73605c4a42d38753a83800f7370f\",\"Header\":\"Marriage+Date\",\"Selectable\":false,\"Selected\":false,\"Json\":false,\"DataType\":\"string\",\"QueryDataType\":null},{\"Id\":\"SacramentVPESacramenta2aa73605c4a42d38753a83800f7370f\",\"Name\":\"Sacrament.SubSacramentVPESacramenta2aa73605c4a42d38753a83800f7370f\",\"Header\":\"Volume,+Page,+Entry\",\"Selectable\":false,\"Selected\":false,\"Json\":false,\"DataType\":\"string\",\"QueryDataType\":null},{\"Id\":\"\",\"Name\":\"Sacrament.Sacrament0b5320cca5154d929b70a83800f73713\",\"Header\":\"Bible+Milestone+Completed?\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":4},{\"Id\":\"SubSacramentDateSacrament0b5320cca5154d929b70a83800f73713\",\"Name\":\"Sacrament.SubSacramentDateSacrament0b5320cca5154d929b70a83800f73713\",\"Header\":\"Bible+Milestone+Date\",\"Selectable\":false,\"Selected\":false,\"Json\":false,\"DataType\":\"string\",\"QueryDataType\":null},{\"Id\":\"SacramentVPESacrament0b5320cca5154d929b70a83800f73713\",\"Name\":\"Sacrament.SubSacramentVPESacrament0b5320cca5154d929b70a83800f73713\",\"Header\":\"Volume,+Page,+Entry\",\"Selectable\":false,\"Selected\":false,\"Json\":false,\"DataType\":\"string\",\"QueryDataType\":null},{\"Id\":\"\",\"Name\":\"Sacrament.Sacrament0f9d5ddab684481ba216a83800f73713\",\"Header\":\"Child+Dedication+Completed?\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":4},{\"Id\":\"SubSacramentDateSacrament0f9d5ddab684481ba216a83800f73713\",\"Name\":\"Sacrament.SubSacramentDateSacrament0f9d5ddab684481ba216a83800f73713\",\"Header\":\"Child+Dedication+Date\",\"Selectable\":false,\"Selected\":false,\"Json\":false,\"DataType\":\"string\",\"QueryDataType\":null},{\"Id\":\"SacramentVPESacrament0f9d5ddab684481ba216a83800f73713\",\"Name\":\"Sacrament.SubSacramentVPESacrament0f9d5ddab684481ba216a83800f73713\",\"Header\":\"Volume,+Page,+Entry\",\"Selectable\":false,\"Selected\":false,\"Json\":false,\"DataType\":\"string\",\"QueryDataType\":null},{\"Id\":\"\",\"Name\":\"AddedWhen\",\"Header\":\"Added+When\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"DateTime\",\"QueryDataType\":9},{\"Id\":\"\",\"Name\":\"Profilee8f9502e0f304004b30ea82301001bb8\",\"Header\":\"Anniversary+Date\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"DateTime\",\"QueryDataType\":1},{\"Id\":\"\",\"Name\":\"SystemAllergies\",\"Header\":\"Allergies+&+Medical+Concerns\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":6},{\"Id\":\"\",\"Name\":\"Birthday\",\"Header\":\"Date+of+Birth\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"DateTime\",\"QueryDataType\":1},{\"Id\":\"\",\"Name\":\"BirthdayDay\",\"Header\":\"Date+of+Birth+(Day)\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"Int32\",\"QueryDataType\":2},{\"Id\":\"\",\"Name\":\"BirthdayMonth\",\"Header\":\"Date+of+Birth+(Month)\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"Int32\",\"QueryDataType\":6},{\"Id\":\"\",\"Name\":\"BirthdayYear\",\"Header\":\"Date+of+Birth+(Year)\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"Int32\",\"QueryDataType\":2},{\"Id\":\"\",\"Name\":\"Profile5972f7a91ca640e8ba28ae2a016dfabe\",\"Header\":\"Do+Not+Email\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"Boolean\",\"QueryDataType\":4},{\"Id\":\"\",\"Name\":\"SystemGender\",\"Header\":\"Gender\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":6},{\"Id\":\"\",\"Name\":\"Profileb4bfccedfdaa4f02a466a95701014136\",\"Header\":\"HS+Graduation+Year\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"DateTime\",\"QueryDataType\":1},{\"Id\":\"\",\"Name\":\"Profile1a419a7e62c04bf9a7c5acf3014e0cf3\",\"Header\":\"Insurance+Carrier\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":0},{\"Id\":\"\",\"Name\":\"Profile596cbbc385934da58ce4acf3014e42de\",\"Header\":\"Insurance+Policy+Number\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":0},{\"Id\":\"\",\"Name\":\"BackgroundCheckDate\",\"Header\":\"Last+Background+Check+Date\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"DateTime\",\"QueryDataType\":1},{\"Id\":\"\",\"Name\":\"BackgroundCheckPackageName\",\"Header\":\"Last+Background+Check+Package+Name\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":0},{\"Id\":\"\",\"Name\":\"BackgroundCheckStatus\",\"Header\":\"Last+Background+Check+Status\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":6},{\"Id\":\"\",\"Name\":\"Profilec32cdb0338484ca1b254af5d01359b20\",\"Header\":\"Maiden+Name\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":0},{\"Id\":\"\",\"Name\":\"Profileb2db6ff2c2514f8f8815ae540144a808\",\"Header\":\"Mandated+Reporter+Completion\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"DateTime\",\"QueryDataType\":1},{\"Id\":\"\",\"Name\":\"Profileadefa2261f1243b8b450ae540144bc0d\",\"Header\":\"Mandated+Reporter+Expiration\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"DateTime\",\"QueryDataType\":1},{\"Id\":\"\",\"Name\":\"SystemMaritalStatus\",\"Header\":\"Marital+Status\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":6},{\"Id\":\"\",\"Name\":\"SystemStatus\",\"Header\":\"Member+Status\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"String\",\"QueryDataType\":6},{\"Id\":\"\",\"Name\":\"Profile51529dc0217d495891fdacca0173dcf3\",\"Header\":\"Rooted+Graduation+Date\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"DateTime\",\"QueryDataType\":1},{\"Id\":\"\",\"Name\":\"Profile3810d9ab461a40f2b3b0ae51000d8d75\",\"Header\":\"No+Regrets+Gold+Graduate\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"DateTime\",\"QueryDataType\":1},{\"Id\":\"\",\"Name\":\"Profile23e1a5b107a84bf6a16eae51000d5294\",\"Header\":\"No+Regrets+Silver+graduate\",\"Selectable\":true,\"Selected\":true,\"Json\":false,\"DataType\":\"DateTime\",\"QueryDataType\":1}]" ),
                new KeyValuePair<string, string>( "query-selected-columns", "[]" ),
                new KeyValuePair<string, string>( "query-report-type", "IndividualLabels" ),
                new KeyValuePair<string, string>( "sort-by", "" ),
                new KeyValuePair<string, string>( "sort-ascending", "true" ),
                new KeyValuePair<string, string>( "report-name", "" ),
                new KeyValuePair<string, string>( "custom-selection", "false" ),
                new KeyValuePair<string, string>( "__RequestVerificationToken", header_ResponseVerificationToken! )
            };

        var cookieContainer = new CookieContainer();
        cookieContainer.Add( new Uri( "https://onrealm.org" ), new Cookie( "StratusWeb", cookie_StatusWeb ) );
        cookieContainer.Add( new Uri( "https://onrealm.org" ), new Cookie( "__RequestVerificationToken", cookie_RequestVerificationToken ) );

        var handler = new HttpClientHandler() { CookieContainer = cookieContainer };

        HttpClient httpClient = new HttpClient( handler );

        IEnumerable<Individual> individuals = new List<Individual>();
        try
        {
            var req = new HttpRequestMessage( HttpMethod.Post, "https://onrealm.org/FriendsChurch/Queries/ExportResults" ) { Content = new FormUrlEncodedContent( nvc ) };

            var response = await httpClient.SendAsync( req );
            var reader = new StreamReader( response.Content.ReadAsStream() );

            var csv = new CsvReader( reader, new CsvConfiguration { CultureInfo = CultureInfo.InvariantCulture } );
            csv.Configuration.RegisterClassMap<IndividualClassMap>();
            individuals = csv.GetRecords<Individual>();
        }
        catch ( Exception ex )
        {
            Console.WriteLine( ex );
        }


        foreach ( var item in individuals )
        {
            yield return item;
        }



    }
}