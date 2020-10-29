﻿using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using Slingshot.Core;
using Slingshot.Core.Model;
using Slingshot.Core.Utilities;
using Slingshot.PCO.Models.ApiModels;
using Slingshot.PCO.Models.DTO;
using Slingshot.PCO.Utilities.Translators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace Slingshot.PCO.Utilities
{
    /// <summary>
    /// PCO API Class.
    /// </summary>
    public static class PCOApi
    {
        private static RestClient _client;
        private static RestRequest _request;

        #region Properties

        /// <summary>
        /// Gets or sets the number of seconds the api was throttled by rate limiting.
        /// </summary>
        public static int ApiThrottleSeconds { get; private set; } = 0;

        /// <summary>
        /// Gets or sets the api counter.
        /// </summary>
        /// <value>
        /// The api counter.
        /// </value>
        public static int ApiCounter { get; set; } = 0;

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public static string ErrorMessage { get; set; }

        /// <summary>
        /// Gets the API URL.
        /// </summary>
        /// <value>
        /// The API URL.
        /// </value>
        public static string ApiBaseUrl
        {
            get
            {
                return $"https://api.planningcenteronline.com";
            }
        }

        /// <summary>
        /// Gets or sets the API consumer key.
        /// </summary>
        /// <value>
        /// The API consumer key.
        /// </value>
        public static string ApiConsumerKey { get; set; }

        /// <summary>
        /// Gets or sets the API consumer secret.
        /// </summary>
        /// <value>
        /// The API consumer secret.
        /// </value>
        public static string ApiConsumerSecret { get; set; }

        /// <summary>
        /// Is Connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if the connected; otherwise, <c>false</c>.
        /// </value>
        public static bool IsConnected { get; private set; } = false;

        #endregion Properties

        /// <summary>
        /// Api Endpoint Declarations.
        /// </summary>
        internal static class ApiEndpoint
        {
            internal const string API_MYSELF = "/people/v2/me";
            internal const string API_PEOPLE = "/people/v2/people";
            internal const string API_SERVICE_PEOPLE = "/services/v2/people";
            internal const string API_NOTES = "/people/v2/notes";
            internal const string API_FIELD_DEFINITIONS = "/people/v2/field_definitions";
            internal const string API_FUNDS = "/giving/v2/funds";
            internal const string API_BATCHES = "/giving/v2/batches";
            internal const string API_DONATIONS = "/giving/v2/donations";
            internal const string API_GROUPTYPES = "/groups/v2/group_types";
            internal const string API_GROUPS = "/groups/v2/group_types/{groupTypeId}/groups";
            internal const string API_GROUPMEMBERS = "/groups/v2/groups/{groupId}/memberships";
            internal const string API_GROUPTAGGROUPS = "/groups/v2/tag_groups";
            internal const string API_GROUPTAGS = "/groups/v2/groups/{groupId}/tags";
            internal const string API_GROUPLOCATIONS = "/groups/v2/groups/{groupId}/location";
            internal const string API_GROUPEVENTS = "/groups/v2/groups/{groupId}/events";
            internal const string API_GROUPATTENDANCE = "/groups/v2/events/{eventId}/attendances";
        }

        /// <summary>
        /// Initializes the export.
        /// </summary>
        public static void InitializeExport()
        {
            PCOApi.ErrorMessage = string.Empty;
            PCOApi.ApiThrottleSeconds = 0;
            ImportPackage.InitalizePackageFolder();
        }

        /// <summary>
        /// Connects to the PCO API.
        /// </summary>
        /// <param name="apiUsername">The API username.</param>
        /// <param name="apiPassword">The API password.</param>
        public static void Connect( string apiConsumerKey, string apiConsumerSecret )
        {
            ApiConsumerKey = apiConsumerKey;
            ApiConsumerSecret = apiConsumerSecret;
            ApiCounter = 0;

            var response = ApiGet( ApiEndpoint.API_MYSELF );
            IsConnected = ( response != string.Empty );
        }

        #region Private Data Access Methods

        /// <summary>
        /// Issues a GET request to the PCO API for the specified end point and returns the response.
        /// </summary>
        /// <param name="apiEndpoint">The API end point.</param>
        /// <param name="apiRequestOptions">An optional collection of request options.</param>
        /// <param name="ignoreApiErrors">[true] if API errors should be ignored.</param>
        /// <returns></returns>
        private static string ApiGet( string apiEndpoint, Dictionary<string, string> apiRequestOptions = null, bool ignoreApiErrors = false )
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var fullApiUrl = ApiBaseUrl + apiEndpoint + GetRequestQueryString( apiRequestOptions );
                _client = new RestClient( fullApiUrl );
                _client.Authenticator = new HttpBasicAuthenticator( ApiConsumerKey, ApiConsumerSecret );

                _request = new RestRequest( Method.GET );
                _request.AddHeader( "accept", "application/json" );

                var response = _client.Execute( _request );

                ApiCounter++;

                if ( response.StatusCode == HttpStatusCode.OK )
                {
                    return response.Content;
                }
                else if ( response.StatusCode == HttpStatusCode.Forbidden && !ignoreApiErrors)
                {
                    throw new Exception( $"Forbidden request: {apiEndpoint}" );
                }
                else if ( ( int ) response.StatusCode == 429 )
                {
                    // If we've got a 'too many requests' error, delay for a number of seconds specified by 'Retry-After

                    var retryAfter = response.Headers
                        .Where( h => h.Name.Equals( "Retry-After", StringComparison.InvariantCultureIgnoreCase ) )
                        .Select( x => ( ( string ) x.Value ).AsIntegerOrNull() )
                        .FirstOrDefault();

                    if ( !retryAfter.HasValue && !ignoreApiErrors )
                    {
                        throw new Exception( "Received HTTP 429 response without 'Retry-After' header." );
                    }

                    int waitTime = ( retryAfter.Value * 1000 ) + 50; // Add 50ms to avoid clock synchronization issues.
                    PCOApi.ApiThrottleSeconds += retryAfter.Value;
                    Thread.Sleep( waitTime );

                    return ApiGet( apiEndpoint, apiRequestOptions, ignoreApiErrors );
                }

                // If we made it here, the response can be assumed to be an error.
                if ( !ignoreApiErrors )
                {
                    PCOApi.ErrorMessage = response.StatusCode + ": " + response.Content;
                }
            }
            catch ( Exception ex )
            {
                if ( !ignoreApiErrors )
                {
                    PCOApi.ErrorMessage = ex.Message;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Converts a dictionary into a formatted query string.
        /// </summary>
        /// <param name="apiRequestOptions"></param>
        /// <returns></returns>
        private static string GetRequestQueryString( Dictionary<string, string> apiRequestOptions )
        {
            var requestQueryString = new System.Text.StringBuilder();
            apiRequestOptions = apiRequestOptions ?? new Dictionary<string, string>();

            foreach( string key in apiRequestOptions.Keys )
            {
                if ( requestQueryString.Length > 0 )
                {
                    requestQueryString.Append( "&" );
                }
                else
                {
                    requestQueryString.Append( "?" );
                }

                var name = WebUtility.UrlEncode( key );
                var value = WebUtility.UrlEncode( apiRequestOptions[key] );

                requestQueryString.Append( $"{ name }={ value }" );
            }

            return requestQueryString.ToString();
        }

        /// <summary>
        /// Gets the results of an API query for the specified API end point.
        /// </summary>
        /// <param name="apiEndpoint">The API end point.</param>
        /// <param name="apiRequestOptions">An optional collection of request options.</param>
        /// <param name="modifiedSince">The modified since.</param>
        /// <param name="existingResults">Previous results for this request that should be combined (for paging purposes).</param>
        /// <param name="ignoreApiErrors">[true] if API errors should be ignored.</param>
        /// <returns></returns>
        private static PCOApiQueryResult GetAPIQuery( string apiEndpoint, Dictionary<string, string> apiRequestOptions = null, DateTime? modifiedSince = null, PCOApiQueryResult existingResults = null, bool ignoreApiErrors = false )
        {
            if ( modifiedSince.HasValue && apiRequestOptions != null )
            {
                // Add a parameter to sort records by last update, descending.
                apiRequestOptions.Add( "order", "-updated_at" );
            }

            string result = ApiGet( apiEndpoint, apiRequestOptions, ignoreApiErrors );
            if ( result.IsNullOrWhiteSpace() )
            {
                return null;
            }

            result = result.CleanResult();

            var itemsResult = JsonConvert.DeserializeObject<QueryItems>( result );
            if ( itemsResult == null )
            {
                PCOApi.ErrorMessage = $"Error:  Unable to deserialize result retrieved from { apiEndpoint }.";
                throw new Exception( PCOApi.ErrorMessage );
            }


            PCOApiQueryResult queryResult;
            if ( existingResults != null )
            {
                queryResult = new PCOApiQueryResult( existingResults, itemsResult.IncludedItems );
            }
            else
            {
                queryResult = new PCOApiQueryResult( itemsResult.IncludedItems );
            }

            // Loop through each item in the results
            var continuePaging = true;
            foreach ( var itemResult in itemsResult.Data )
            {
                // If we're only looking for records updated after last update, and this record is older, stop processing                    
                DateTime? recordUpdatedAt = ( itemResult.Item.updated_at ?? itemResult.Item.created_at );
                if ( modifiedSince.HasValue && recordUpdatedAt.HasValue && recordUpdatedAt.Value <= modifiedSince.Value )
                {
                    continuePaging = false;
                    break;
                }

                queryResult.Items.Add( itemResult );
            }

            // If there are more page, and we should be paging
            string nextEndpoint = itemsResult.Links != null && itemsResult.Links.Next != null ? itemsResult.Links.Next : string.Empty;
            if ( nextEndpoint.IsNotNullOrWhitespace() && continuePaging )
            {
                nextEndpoint = nextEndpoint.Substring( ApiBaseUrl.Length );
                // Get the next page of results by doing a recursive call to this same method.
                // Note that nextEndpoint is supplied without the options dictionary, as those should already be specified in the result from PCO.
                return GetAPIQuery( nextEndpoint, null, modifiedSince, queryResult );
            }

            return queryResult;
        }

        /// <summary>
        /// Replaces id values of "unique" with -1 to allow JSON deserialization.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private static string CleanResult( this string result )
        {
            return result.Replace( "\"id\":\"unique\"", "\"id\":\"-1\"" );
        }

        #endregion Private Data Access Methods

        #region ExportIndividuals() and Related Methods

        /// <summary>
        /// Exports the individuals.
        /// </summary>
        /// <param name="modifiedSince">The modified since.</param>
        /// <param name="peoplePerPage">The people per page.</param>
        public static void ExportIndividuals( DateTime modifiedSince, int peoplePerPage = 100 )
        {
            var apiOptions = new Dictionary<string, string>
            {
                { "include", "emails,addresses,phone_numbers,field_data,households,inactive_reason,martial_status,name_prefix,name_suffix,primary_campus,school,social_profiles" },
                { "per_page", peoplePerPage.ToString() }
            };

            var PCOPeople = GetPeople( ApiEndpoint.API_PEOPLE, apiOptions, modifiedSince );
            var PCOServicePeople = GetServicePeople( modifiedSince );
            var PCONotes = GetNotes( modifiedSince );
            var headOfHouseholdMap = GetHeadOfHouseholdMap( PCOPeople );
            var personAttributes = WritePersonAttributes();

            foreach ( var person in PCOPeople )
            {
                PersonDTO headOfHouse = person; // Default headOfHouse to person, in case they are not assigned to a household in PCO.
                if( person.Household != null && headOfHouseholdMap.ContainsKey( person.Household.Id ) )
                {
                    headOfHouse = headOfHouseholdMap[person.Household.Id];
                }

                // The backgroundCheckPerson is pulled from a different API endpoint.
                PersonDTO backgroundCheckPerson = null;
                if ( PCOServicePeople != null )
                {
                    backgroundCheckPerson = PCOServicePeople.Where( x => x.Id == person.Id ).FirstOrDefault();
                }

                var importPerson = PCOImportPerson.Translate( person, personAttributes, headOfHouse, backgroundCheckPerson );
                if ( importPerson != null )
                {
                    ImportPackage.WriteToPackage( importPerson );
                }

                // save person image
                if ( person.Avatar.IsNotNullOrWhitespace() )
                {
                    WebClient client = new WebClient();

                    var path = Path.Combine( ImportPackage.ImageDirectory, "Person_" + person.Id + ".png" );
                    try
                    {
                        client.DownloadFile( new Uri( person.Avatar ), path );
                        ApiCounter++;
                    }
                    catch ( Exception ex )
                    {
                        Console.WriteLine( ex.Message );
                    }
                }
            }
            // save notes.
            if ( PCONotes != null )
            {
                foreach ( NoteDTO note in PCONotes )
                {
                    PersonNote importNote = PCOImportPersonNote.Translate( note );
                    if ( importNote != null )
                    {
                        ImportPackage.WriteToPackage( importNote );
                    }
                }
            }
        }

        /// <summary>
        /// Gets people from the services endpoint.
        /// </summary>
        /// <param name="modifiedSince">The modified since.</param>
        /// <param name="peoplePerPage">The people per page.</param>
        /// <returns></returns>
        private static List<PersonDTO> GetServicePeople( DateTime modifiedSince, int peoplePerPage = 100 )
        {
            var apiOptions = new Dictionary<string, string>
            {
                { "per_page", peoplePerPage.ToString() }
            };

            return GetPeople( ApiEndpoint.API_SERVICE_PEOPLE, apiOptions, modifiedSince );
        }

        /// <summary>
        /// Gets people from the specified endpoint.
        /// </summary>
        /// <param name="apiEndPoint">The API end point.</param>
        /// <param name="apiRequestOptions">A collection of request options.</param>
        /// <param name="modifiedSince">The modified since.</param>
        /// <returns></returns>
        public static List<PersonDTO> GetPeople( string apiEndPoint, Dictionary<string, string> apiRequestOptions, DateTime? modifiedSince )
        {
            var people = new List<PersonDTO>();

            var personQuery = GetAPIQuery( apiEndPoint, apiRequestOptions, modifiedSince );

            if ( personQuery == null )
            {
                return people;
            }

            foreach ( var item in personQuery.Items )
            {
                var person = new PersonDTO( item, personQuery.IncludedItems );
                people.Add( person );
            }

            return people;
        }

        /// <summary>
        /// Maps household Ids to the PCOPerson object designated as the primary contact for that household.  This map method is used to avoid repetitive searches for the head of household for each household member.
        /// </summary>
        /// <param name="people">The list of <see cref="PersonDTO"/> records.</param>
        /// <returns></returns>
        private static Dictionary<int, PersonDTO> GetHeadOfHouseholdMap( List<PersonDTO> people )
        {
            var map = new Dictionary<int, PersonDTO>();

            foreach ( var person in people )
            {
                if ( person.Household == null || map.ContainsKey( person.Household.Id ) )
                {
                    continue;
                }

                if ( person.Household.PrimaryContactId == person.Id )
                {
                    map.Add( person.Household.Id, person );
                }
            }

            return map;
        }

        /// <summary>
        /// Gets notes from PCO.
        /// </summary>
        /// <param name="modifiedSince">The modified since.</param>
        /// <returns></returns>
        public static List<NoteDTO> GetNotes( DateTime? modifiedSince )
        {
            var notes = new List<NoteDTO>();

            var apiOptions = new Dictionary<string, string>
            {
                { "include", "category" }
            };

            var notesQuery = GetAPIQuery( ApiEndpoint.API_NOTES, apiOptions, modifiedSince );

            if ( notesQuery == null )
            {
                return notes;
            }


            foreach ( var item in notesQuery.Items )
            {
                var note = new NoteDTO( item, notesQuery.IncludedItems );
                notes.Add( note );
            }

            return notes;
        }

        /// <summary>
        /// Exports the person attributes.
        /// </summary>
        private static List<FieldDefinitionDTO> WritePersonAttributes()
        {
            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "Facebook",
                Key = "Facebook",
                Category = "Social Media",
                FieldType = "Rock.Field.Types.SocialMediaAccountFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "Twitter",
                Key = "Twitter",
                Category = "Social Media",
                FieldType = "Rock.Field.Types.SocialMediaAccountFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "Instagram",
                Key = "Instagram",
                Category = "Social Media",
                FieldType = "Rock.Field.Types.SocialMediaAccountFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "LinkedIn",
                Key = "LinkedIn",
                Category = "Social Media",
                FieldType = "Rock.Field.Types.SocialMediaAccountFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "School",
                Key = "School",
                Category = "Education",
                FieldType = "Rock.Field.Types.TextFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "PCO Remote Id",
                Key = "RemoteId",
                Category = "Education",
                FieldType = "Rock.Field.Types.TextFieldType"
            } );

            ImportPackage.WriteToPackage( new PersonAttribute()
            {
                Name = "Background Check Result",
                Key = "BackgroundCheckResult",
                Category = "Safety & Security",
                FieldType = "Rock.Field.Types.TextFieldType"
            } );

            var attributes = new List<FieldDefinitionDTO>();

            // export person attributes
            try
            {
                var fieldDefinitions = GetFieldDefinitions();

                foreach ( var fieldDefinition in fieldDefinitions )
                {
                    // get field type
                    var fieldtype = "Rock.Field.Types.TextFieldType";
                    if ( fieldDefinition.DataType == "text" )
                    {
                        fieldtype = "Rock.Field.Types.MemoFieldType";
                    }
                    else if ( fieldDefinition.DataType == "date" )
                    {
                        fieldtype = "Rock.Field.Types.DateFieldType";
                    }
                    else if ( fieldDefinition.DataType == "boolean" )
                    {
                        fieldtype = "Rock.Field.Types.BooleanFieldType";
                    }
                    else if ( fieldDefinition.DataType == "file" )
                    {
                        continue;
                        //fieldtype = "Rock.Field.Types.FileFieldType";
                    }
                    else if ( fieldDefinition.DataType == "number" )
                    {
                        fieldtype = "Rock.Field.Types.IntegerFieldType";
                    }

                    var newAttribute = new PersonAttribute()
                    {
                        Name = fieldDefinition.Name,
                        Key = fieldDefinition.Id + "_" + fieldDefinition.Slug,
                        Category = ( fieldDefinition.Tab == null ) ? "PCO Attributes" : fieldDefinition.Tab.Name,
                        FieldType = fieldtype,
                    };

                    ImportPackage.WriteToPackage( newAttribute );

                    attributes.Add( fieldDefinition );
                }
            }
            catch ( Exception ex )
            {
                ErrorMessage = ex.Message;
            }

            return attributes;
        }

        /// <summary>
        /// Get the field definitions from PCO.
        /// </summary>
        /// <returns></returns>
        private static List<FieldDefinitionDTO> GetFieldDefinitions()
        {
            var fields = new List<FieldDefinitionDTO>();

            var apiOptions = new Dictionary<string, string>
            {
                { "include", "field_options,tab" }
            };

            var fieldQuery = GetAPIQuery( ApiEndpoint.API_FIELD_DEFINITIONS, apiOptions );

            if ( fieldQuery == null )
            {
                return fields;
            }

            foreach ( var item in fieldQuery.Items )
            {
                var field = new FieldDefinitionDTO( item, fieldQuery.IncludedItems );
                if ( field != null && field.DataType != "header" )
                {
                    fields.Add( field );
                }
            }
            return fields;
        }

        #endregion ExportIndividuals() and Related Methods

        #region ExportFinancialAccounts() and Related Methods

        /// <summary>
        /// Exports the accounts.
        /// </summary>
        public static void ExportFinancialAccounts()
        {
            try
            {
                var funds = GetFunds();

                foreach ( var fund in funds )
                {
                    var importFund = PCOImportFund.Translate( fund );
                    if ( importFund != null )
                    {
                        ImportPackage.WriteToPackage( importFund );
                    }
                }
            }
            catch ( Exception ex )
            {
                ErrorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Get the Funds from PCO.
        /// </summary>
        /// <returns></returns>
        private static List<FundDTO> GetFunds()
        {
            var funds = new List<FundDTO>();

            var fundQuery = GetAPIQuery( ApiEndpoint.API_FUNDS );

            if ( fundQuery == null )
            {
                return funds;
            }

            foreach ( var item in fundQuery.Items )
            {
                var fund = new FundDTO( item );
                if ( fund != null )
                {
                    funds.Add( fund );
                }
            }

            return funds;
        }

        #endregion ExportFinancialAccounts() and Related Methods

        #region ExportFinancialBatches() and Related Methods

        /// <summary>
        /// Exports the batches.
        /// </summary>
        /// <param name="modifiedSince">The modified since.</param>
        public static void ExportFinancialBatches( DateTime modifiedSince )
        {
            try
            {
                var batches = GetBatches( modifiedSince );

                foreach ( var batch in batches )
                {
                    var importBatch = PCOImportBatch.Translate( batch );

                    if ( importBatch != null )
                    {
                        ImportPackage.WriteToPackage( importBatch );
                    }
                }

            }
            catch ( Exception ex )
            {
                ErrorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Get the Batches from PCO.
        /// </summary>
        /// <param name="modifiedSince">The modified since.</param>
        /// <returns></returns>
        public static List<BatchDTO> GetBatches( DateTime? modifiedSince )
        {
            var batches = new List<BatchDTO>();

            var apiOptions = new Dictionary<string, string>
            {
                { "include", "owner" },
                { "per_page", "100" }
            };

            var batchesQuery = GetAPIQuery( ApiEndpoint.API_BATCHES, apiOptions, modifiedSince );

            if ( batchesQuery == null )
            {
                return batches;
            }

            foreach ( var item in batchesQuery.Items )
            {
                var batch = new BatchDTO( item );
                if ( batch != null )
                {
                    batches.Add( batch );
                }
            }

            return batches;
        }

        #endregion ExportFinancialBatches() and Related Methods

        #region ExportContributions() and Related Methods

        /// <summary>
        /// Exports the contributions.
        /// </summary>
        /// <param name="modifiedSince">The modified since.</param>
        public static void ExportContributions( DateTime modifiedSince )
        {
            try
            {
                var donations = GetDonations( modifiedSince )
                    .Where( d => d.Refunded == false && d.PaymentStatus == "succeeded" )
                    .ToList();

                foreach ( var donation in donations )
                {
                    var importTransaction = PCOImportDonation.Translate( donation );
                    var importTransactionDetails = PCOImportDesignation.Translate( donation );

                    if ( importTransaction != null )
                    {
                        ImportPackage.WriteToPackage( importTransaction );

                        foreach( var detail in importTransactionDetails )
                        {
                            if ( detail != null )
                            {
                                ImportPackage.WriteToPackage( detail );
                            }
                        }
                    }                                
                }            
            }
            catch ( Exception ex )
            {
                ErrorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Gets Donations from PCO.
        /// </summary>
        /// <param name="modifiedSince">The modified since.</param>
        /// <returns></returns>
        private static List<DonationDTO> GetDonations( DateTime? modifiedSince )
        {
            var donations = new List<DonationDTO>();

            var apiOptions = new Dictionary<string, string>
            {
                { "include", "designations" },
                { "per_page", "100" }
            };

            var donationsQuery = GetAPIQuery( ApiEndpoint.API_DONATIONS, apiOptions, modifiedSince );

            if ( donationsQuery == null )
            {
                return donations;
            }

            foreach ( var item in donationsQuery.Items )
            {
                var donation = new DonationDTO( item, donationsQuery.IncludedItems );
                if ( donation != null )
                {
                    donations.Add( donation );
                }
            }

            return donations;
        }

        #endregion ExportContributions() and Related Methods

        #region ExportGroups() and Related Methods

        /// <summary>
        /// Gets the group types.
        /// </summary>
        /// <returns></returns>
        public static List<GroupTypeDTO> GetGroupTypes()
        {
            var groupTypes = new List<GroupTypeDTO>();

            var apiOptions = new Dictionary<string, string>
            {
                { "order", "name" },
                { "per_page", "100" }
            };

            var groupTypeQuery = GetAPIQuery( ApiEndpoint.API_GROUPTYPES, apiOptions );

            if ( groupTypeQuery == null )
            {
                return groupTypes;
            }

            foreach ( var item in groupTypeQuery.Items )
            {
                var groupType = new GroupTypeDTO( item );
                if ( groupType != null )
                {
                    groupTypes.Add( groupType );
                }
            }

            return groupTypes;
        }

        /// <summary>
        /// Exports the groups.
        /// </summary>
        /// <param name="modifiedSince">The modified since.</param>
        /// <param name="exportGroupTypes">The list of <see cref="GroupTypeDTO"/>s to export.</param>
        public static void ExportGroups( List<GroupTypeDTO> exportGroupTypes )
        {
            try
            {
                // Create Group Attributes from Tag Groups.
                var tagGroups = GetGroupTagGroups();
                foreach ( var tagGroup in tagGroups )
                {
                    var groupAttribute = PCOImportGroupAttribute.Translate( tagGroup );
                    ImportPackage.WriteToPackage( groupAttribute );
                }

                var groupTypes = GetGroupTypes();

                // The special "unique" group type in PCO needs to have an integer value assigned, so we will find the highest number currently used and add 1.
                int maxGroupType = 0;
                foreach ( var groupType in exportGroupTypes )
                {
                    maxGroupType = ( groupType.Id > maxGroupType ) ? groupType.Id : maxGroupType;
                }

                // Export each group type.
                foreach ( var groupType in exportGroupTypes )
                {
                    if ( groupType.Id == -1 )
                    {
                        // "Unique" group type.
                        groupType.Id = maxGroupType += 1;
                        ExportGroupType( groupType, tagGroups, true );
                    }
                    else
                    {
                        ExportGroupType( groupType, tagGroups );
                    }
                }
            }
            catch ( Exception ex )
            {
                ErrorMessage = ex.Message;
            }
        }

        private static void ExportGroupType( GroupTypeDTO groupType, List<TagGroupDTO> tagGroups, bool isUnique = false )
        {
            // Write the GroupType.
            var exportGroupType = PCOImportGroupType.Translate( groupType );
            ImportPackage.WriteToPackage( exportGroupType );

            // Iterate over each Group in the GroupType.
            var groups = GetGroups( groupType, isUnique );
            foreach ( var group in groups )
            {
                var importGroup = PCOImportGroup.Translate( group );
                if ( importGroup != null )
                {
                    ImportPackage.WriteToPackage( importGroup );

                    // Export GroupMembers.
                    ExportGroupMembers( importGroup );

                    // Export Group AttributeValues from Tags.
                    ExportGroupTags( importGroup, tagGroups );

                    // Export GroupAddresses.
                    if ( group.HasLocation )
                    {
                        ExportGroupLocations( importGroup );
                    }

                    // Export Attendance.
                    ExportGroupAttendance( importGroup );
                }
            }
        }

        private static void ExportGroupMembers( Group importGroup )
        {
            var groupMembers = GetGroupMembers( importGroup.Id );
            foreach ( var groupMember in groupMembers )
            {
                var importGroupMember = PCOImportGroupMember.Translate( groupMember );
                if ( importGroupMember != null )
                {
                    ImportPackage.WriteToPackage( importGroupMember );
                }
            }
        }

        private static void ExportGroupTags( Group importGroup, List<TagGroupDTO> tagGroups )
        {
            // Each tag becomes a comma-separated value in an attribute keyed to the tag group.
            var groupTags = GetGroupTags( importGroup.Id, tagGroups );
            var groupedGroupTags = new Dictionary<string, List<TagDTO>>();
            foreach( var groupTag in groupTags )
            {
                string attributeKey = groupTag.TagGroup.GroupAttributeKey;
                if ( groupedGroupTags.ContainsKey( attributeKey ) )
                {
                    groupedGroupTags[attributeKey].Add( groupTag );
                }
                else
                {
                    groupedGroupTags.Add( attributeKey, new List<TagDTO>() { groupTag } );
                }
            }
            
            // Write the Group AttributeValues.
            foreach ( var attributeKey in groupedGroupTags.Keys )
            {
                var values = new List<string>();
                var groupedTags = groupedGroupTags[attributeKey];

                // Combine all of the tag values.
                foreach ( var groupTag in groupedTags )
                {
                    values.Add( groupTag.GroupAttributeValue );
                }

                ImportPackage.WriteToPackage(
                    new GroupAttributeValue()
                    {
                        AttributeKey = attributeKey,
                        AttributeValue = values.ToDelimited(),
                        GroupId = importGroup.Id
                    } );
            }
        }

        private static string ToDelimited( this List<string> input )
        {
            string output = string.Empty;
            foreach ( var value in input )
            {
                if ( output != string.Empty )
                {
                    output += ",";
                }

                output += value;
            }

            return output;
        }

        private static void ExportGroupLocations( Group importGroup )
        {
            var groupLocations = GetGroupLocations( importGroup.Id );
            foreach ( var groupLocation in groupLocations )
            {
                var groupAddress = PCOImportGroupAddress.Translate( groupLocation, importGroup.Id );
                if ( groupAddress != null )
                {
                    ImportPackage.WriteToPackage( groupAddress );
                }
            }
        }

        private static void ExportGroupAttendance( Group importGroup )
        {
            var groupEvents = GetGroupEvents( importGroup.Id );
            foreach ( var groupEvent in groupEvents )
            {
                var groupAttendances = GetGroupAttendance( groupEvent, importGroup.Id );
                foreach ( var groupAttendance in groupAttendances )
                {
                    var importAttendance = PCOImportGroupAttendance.Translate( groupAttendance, importGroup.Id );
                    if ( importAttendance != null )
                    {
                        ImportPackage.WriteToPackage( importAttendance );
                    }
                }

            }
        }

        private static List<GroupDTO> GetGroups( GroupTypeDTO groupType, bool isUnique )
        {
            var groups = new List<GroupDTO>();

            var apiOptions = new Dictionary<string, string>
            {
                { "per_page", "100" }
            };

            string groupTypeId = groupType.Id.ToString();
            if ( isUnique )
            {
                groupTypeId = "unique";
            }

            string groupEndPoint = ApiEndpoint.API_GROUPS.Replace( "{groupTypeId}", groupTypeId );
            var groupQuery = GetAPIQuery( groupEndPoint, apiOptions );

            if ( groupQuery == null )
            {
                return groups;
            }

            foreach ( var item in groupQuery.Items )
            {
                var group = new GroupDTO( item, groupType );
                if ( group != null )
                {
                    groups.Add( group );
                }
            }

            return groups;
        }

        private static List<GroupMemberDTO> GetGroupMembers( int groupId )
        {
            var groupMembers = new List<GroupMemberDTO>();

            var apiOptions = new Dictionary<string, string>
            {
                { "per_page", "100" }
            };

            string groupMemberEndpoint = ApiEndpoint.API_GROUPMEMBERS.Replace( "{groupId}", groupId.ToString() );
            var groupMemberQuery = GetAPIQuery( groupMemberEndpoint, apiOptions );

            if ( groupMemberQuery == null )
            {
                return groupMembers;
            }

            foreach ( var item in groupMemberQuery.Items )
            {
                var groupMember = new GroupMemberDTO( item, groupId );
                if ( groupMember != null )
                {
                    groupMembers.Add( groupMember );
                }
            }

            return groupMembers;
        }

        private static List<TagGroupDTO> GetGroupTagGroups()
        {
            var tagGroups = new List<TagGroupDTO>();

            var apiOptions = new Dictionary<string, string>
            {
                { "per_page", "100" }
            };

            var tagGroupQuery = GetAPIQuery( ApiEndpoint.API_GROUPTAGGROUPS, apiOptions );

            if ( tagGroupQuery == null )
            {
                return tagGroups;
            }

            foreach ( var item in tagGroupQuery.Items )
            {
                var groupMember = new TagGroupDTO( item );
                if ( groupMember != null )
                {
                    tagGroups.Add( groupMember );
                }
            }

            return tagGroups;
        }

        private static List<TagDTO> GetGroupTags( int groupId, List<TagGroupDTO> tagGroups )
        {
            var groupTags = new List<TagDTO>();

            var apiOptions = new Dictionary<string, string>
            {
                { "per_page", "100" }
            };

            string groupMemberEndpoint = ApiEndpoint.API_GROUPTAGS.Replace( "{groupId}", groupId.ToString() );
            var groupTagQuery = GetAPIQuery( groupMemberEndpoint, apiOptions );

            if ( groupTagQuery == null )
            {
                return groupTags;
            }

            foreach ( var item in groupTagQuery.Items )
            {
                var groupTag = new TagDTO( item, tagGroups );
                if ( groupTag != null )
                {
                    groupTags.Add( groupTag );
                }
            }

            return groupTags;
        }

        private static List<LocationDTO> GetGroupLocations( int groupId )
        {
            var groupLocations = new List<LocationDTO>();

            var apiOptions = new Dictionary<string, string>
            {
                { "per_page", "100" }
            };

            string locationEndpoint = ApiEndpoint.API_GROUPLOCATIONS.Replace( "{groupId}", groupId.ToString() );

            /* This request will ignore API errors because the locations endpoint sometimes returns 403 forbidden
             * errors, seemingly at random (it could be a permissions issue in PCO, but the cause is unconfirmed). */

            var groupLocationQuery = GetAPIQuery( locationEndpoint, apiOptions, null, null, true );

            if ( groupLocationQuery == null )
            {
                return groupLocations;
            }

            foreach ( var item in groupLocationQuery.Items )
            {
                var location = new LocationDTO( item );
                if ( location != null && location.IsValid )
                {
                    groupLocations.Add( location );
                }
            }

            return groupLocations;
        }

        private static List<EventDTO> GetGroupEvents( int groupId )
        {
            var groupEvents = new List<EventDTO>();

            var apiOptions = new Dictionary<string, string>
            {
                { "per_page", "100" }
            };

            string locationEndpoint = ApiEndpoint.API_GROUPEVENTS.Replace( "{groupId}", groupId.ToString() );
            var groupEventQuery = GetAPIQuery( locationEndpoint, apiOptions );

            if ( groupEventQuery == null )
            {
                return groupEvents;
            }

            foreach ( var item in groupEventQuery.Items )
            {
                var groupEvent = new EventDTO( item );
                if ( groupEvent != null )
                {
                    groupEvents.Add( groupEvent );
                }
            }

            return groupEvents;
        }

        private static List<AttendanceDTO> GetGroupAttendance( EventDTO groupEvent, int groupId )
        {
            var groupAttendance = new List<AttendanceDTO>();

            var apiOptions = new Dictionary<string, string>
            {
                { "per_page", "100" }
            };

            string locationEndpoint = ApiEndpoint.API_GROUPATTENDANCE.Replace( "{eventId}", groupEvent.Id.ToString() );
            var groupAttendanceQuery = GetAPIQuery( locationEndpoint, apiOptions );

            if ( groupAttendanceQuery == null )
            {
                return groupAttendance;
            }

            foreach ( var item in groupAttendanceQuery.Items )
            {
                var attendance = new AttendanceDTO( item, groupEvent, groupId );
                if ( attendance != null )
                {
                    groupAttendance.Add( attendance );
                }
            }

            return groupAttendance;
        }

        #endregion ExportGroups() and Related Methods
    }
}

