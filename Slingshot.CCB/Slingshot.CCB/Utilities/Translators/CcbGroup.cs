using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Slingshot.Core;
using Slingshot.Core.Model;

namespace Slingshot.CCB.Utilities.Translators
{
    public static class CcbGroup
    {
        public static List<Group> Translate( XElement inputGroup )
        {
            List<Group> groups = new List<Group>();

            int? departmentId = null;
            int? directorId = null;

            var group = new Group();

            group.Id = inputGroup.Attribute( "id" ).Value.AsInteger();
            group.Name = inputGroup.Element( "name" )?.Value;
            group.Description = inputGroup.Element( "description" )?.Value.RemoveCrLf();
            group.GroupTypeId = inputGroup.Element( "group_type" ).Attribute( "id" ).Value.AsInteger();
            group.CampusId = inputGroup.Element( "campus" ).Attribute( "id" ).Value.AsIntegerOrNull();
            group.Capacity = inputGroup.Element( "group_capacity" ).Value.AsIntegerOrNull();
            group.IsActive = !inputGroup.Element( "inactive" ).Value.AsBoolean();
            group.IsPublic = inputGroup.Element( "public_search_listed" ).Value.AsBoolean();
            group.MeetingDay = inputGroup.Element( "meeting_day" ).Value;
            group.MeetingTime = inputGroup.Element( "meeting_time" ).Value;

            ProcessBooleanAttribute( group, inputGroup.Element( "childcare_provided" ), "HasChildcare" );

            if ( group.GroupTypeId == 0 )
            {
                group.GroupTypeId = CcbApi.GROUPTYPE_UNKNOWN_ID;
            }

            groups.Add( group );

            // add the department as a group with an id of 9999 + its id to create a unique group id for it
            string sourceDepartmentId = null;
            if ( inputGroup.Element( "department" ) != null && inputGroup.Element( "department" ).Attribute( "id" ) != null && inputGroup.Element( "department" ).Attribute( "id" ).Value.IsNotNullOrWhitespace() )
            {
                sourceDepartmentId = inputGroup.Element( "department" ).Attribute( "id" ).Value;
                departmentId = ( "9999" + sourceDepartmentId ).AsInteger();
                var departmentName = inputGroup.Element( "department" ).Value;
                if ( departmentName.IsNullOrWhiteSpace() )
                {
                    departmentName = "No Department Name";
                }
                groups.Add( new Group { Id = departmentId.Value, Name = inputGroup.Element( "department" ).Value, IsActive = true, GroupTypeId = 9999 } );
            }

            // add the director as a group. Each department/director pair gets its own group id (see GetDirectorGroupId).
            if ( inputGroup.Element( "director" ) != null && inputGroup.Element( "director" ).Attribute( "id" ) != null && inputGroup.Element( "director" ).Attribute( "id" ).Value.IsNotNullOrWhitespace() )
            {
                directorId = GetDirectorGroupId( group.Id, sourceDepartmentId, inputGroup.Element( "director" ).Attribute( "id" ).Value );
            }

            if ( directorId.HasValue )
            {
                var directorGroup = new Group();
                directorGroup.Id = directorId.Value;
                directorGroup.IsActive = true;
                directorGroup.Name = inputGroup.Element( "director" ).Element( "full_name" ).Value;
                directorGroup.GroupTypeId = 9998;

                // add parent group of the department if it exists
                if ( departmentId.HasValue )
                {
                    directorGroup.ParentGroupId = departmentId.Value;
                }

                directorGroup.GroupMembers.Add( new GroupMember { PersonId = inputGroup.Element( "director" ).Attribute( "id" ).Value.AsInteger(), Role = "Leader", GroupId = directorGroup.Id } );

                groups.Add( directorGroup );
            }

            // add leader
            if ( inputGroup.Element( "main_leader" ).Attribute( "id" ) != null && inputGroup.Element( "main_leader" ).Attribute( "id" ).Value.AsInteger() != 0 )
            {
                group.GroupMembers.Add( new GroupMember { PersonId = inputGroup.Element( "main_leader" ).Attribute( "id" ).Value.AsInteger(), Role = "Leader", GroupId = group.Id } );
            }

            // add assistant leaders
            if ( inputGroup.Element( "leaders" ) != null )
            {
                foreach ( var leaderNode in inputGroup.Element( "leaders" ).Elements( "leader" ) )
                {
                    group.GroupMembers.Add( new GroupMember { PersonId = leaderNode.Attribute( "id" ).Value.AsInteger(), Role = "Assistant Leader", GroupId = group.Id } );
                }
            }

            // add participants
            if ( inputGroup.Element( "participants" ) != null )
            {
                foreach ( var participantNode in inputGroup.Element( "participants" ).Elements( "participant" ) )
                {
                    group.GroupMembers.Add( new GroupMember { PersonId = participantNode.Attribute( "id" ).Value.AsInteger(), Role = "Member", GroupId = group.Id } );
                }
            }

            var addressList = inputGroup.Element( "addresses" ).Elements( "address" );
            foreach ( var address in addressList )
            {
                if ( address.Element( "street_address" ) != null && address.Element( "street_address" ).Value.IsNotNullOrWhitespace() )
                {
                    var importAddress = new GroupAddress();
                    importAddress.GroupId = group.Id;
                    importAddress.Street1 = address.Element( "street_address" ).Value.RemoveCrLf();
                    importAddress.City = address.Element( "city" ).Value;
                    importAddress.State = address.Element( "state" ).Value;
                    importAddress.PostalCode = address.Element( "zip" ).Value;
                    importAddress.Latitude = address.Element( "latitude" )?.Value;
                    importAddress.Longitude = address.Element( "longitude" )?.Value;
                    importAddress.Country = address.Element( "country" )?.Value;

                    var addressType = address.Attribute( "type" ).Value;

                    switch ( addressType )
                    {
                        case "mailing":
                        case "home":
                            {
                                importAddress.AddressType = AddressType.Home;
                                importAddress.IsMailing = addressType.Equals( "mailing" );
                                break;
                            }
                        case "work":
                            {
                                importAddress.AddressType = AddressType.Work;
                                break;
                            }
                        case "other":
                            {
                                importAddress.AddressType = AddressType.Other;
                                break;
                            }
                    }

                    // only add the address if we have a valid address
                    if ( importAddress.Street1.IsNotNullOrWhitespace() && importAddress.City.IsNotNullOrWhitespace() && importAddress.PostalCode.IsNotNullOrWhitespace() )
                    {
                        group.Addresses.Add( importAddress );
                    }
                }
            }

            // determine the parent group
            if ( directorId.HasValue )
            {
                group.ParentGroupId = directorId.Value;
            }
            else if ( departmentId.HasValue )
            {
                group.ParentGroupId = departmentId.Value;
            }

            return groups;
        }

        /*
         * Director group ids
         * ----------------------------------------
         * A director group id is 800,000,000 + ( department id x 1,000,000 ) + director person id.
         *
         * Each department/director pair gets its own group, so a director who covers several departments
         * gets a director group under each one (Spark Slingshot issue #73). Before this, the id was "9998" + director id,
         * so Rock's Bulk Import kept only the first department's director group and every other department's
         * groups landed under it.
         *
         * Spark's fix for #73 (commit 3ab4e1b) joins "9998" + department id + director id as text. For real ids that
         * number is larger than an Int32, AsInteger() silently returns 0, and Rock merges every such director group
         * into one group with no parent. This scheme always fits in an Int32: the largest id is 1,799,999,999.
         *
         * Director group ids (800,000,000 and up) cannot collide with CCB group ids or with department group ids
         * ( "9999" + department id, at most 9,999,999 for department ids up to 999 ).
         *
         * Ids that are not plain digits (no spaces, signs or leading zeros), or are outside the supported range,
         * stop the group export with a DirectorGroupIdOutOfRangeException instead of writing colliding or zero ids.
         * Plain digits only, because the department group id is built from the same raw text, so " 7" or "007"
         * would give a different department group than "7" for the same director group.
         * A director id of 0 means no director, like main_leader. A group with no department and a group in
         * CCB's blank department (id 0) share the same director group.
         */
        private const long DIRECTOR_GROUP_ID_BASE = 800000000;
        private const long DEPARTMENT_ID_MULTIPLIER = 1000000;
        private const long MAX_DEPARTMENT_ID = 999;
        private const long MAX_DIRECTOR_PERSON_ID = 999999;

        /// <summary>
        /// Gets the group id for a director's group within one department.
        /// </summary>
        /// <param name="groupId">The CCB group id (used only in the error message).</param>
        /// <param name="sourceDepartmentId">The CCB department id, or null/blank when the group has no department.</param>
        /// <param name="sourceDirectorId">The CCB person id of the director.</param>
        /// <returns>The director group id, or null when the director id is 0.</returns>
        /// <exception cref="DirectorGroupIdOutOfRangeException">The department or director id is not a number in the supported range.</exception>
        public static int? GetDirectorGroupId( int groupId, string sourceDepartmentId, string sourceDirectorId )
        {
            long directorPersonId;
            if ( !long.TryParse( sourceDirectorId, NumberStyles.None, CultureInfo.InvariantCulture, out directorPersonId )
                || directorPersonId.ToString( CultureInfo.InvariantCulture ) != sourceDirectorId
                || directorPersonId < 0
                || directorPersonId > MAX_DIRECTOR_PERSON_ID )
            {
                throw new DirectorGroupIdOutOfRangeException( $"CCB group {groupId} has director id '{sourceDirectorId}', which is outside the supported range of 0 to {MAX_DIRECTOR_PERSON_ID}. The group export stopped so director groups do not collide; the ccb-export.slingshot from this run is incomplete and must not be imported. Change the id scheme in CcbGroup.GetDirectorGroupId and export again." );
            }

            if ( directorPersonId == 0 )
            {
                return null;
            }

            long departmentNumber = 0;
            if ( sourceDepartmentId.IsNotNullOrWhitespace() )
            {
                if ( !long.TryParse( sourceDepartmentId, NumberStyles.None, CultureInfo.InvariantCulture, out departmentNumber )
                    || departmentNumber.ToString( CultureInfo.InvariantCulture ) != sourceDepartmentId
                    || departmentNumber < 0
                    || departmentNumber > MAX_DEPARTMENT_ID )
                {
                    throw new DirectorGroupIdOutOfRangeException( $"CCB group {groupId} has department id '{sourceDepartmentId}', which is outside the supported range of 0 to {MAX_DEPARTMENT_ID}. The group export stopped so director groups do not collide; the ccb-export.slingshot from this run is incomplete and must not be imported. Change the id scheme in CcbGroup.GetDirectorGroupId and export again." );
                }
            }

            return ( int ) ( DIRECTOR_GROUP_ID_BASE + ( departmentNumber * DEPARTMENT_ID_MULTIPLIER ) + directorPersonId );
        }

        /// <summary>
        /// Processes the string attribute.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="element">The element.</param>
        /// <param name="attributeKey">The attribute key.</param>
        private static void ProcessStringAttribute( Group group, XElement element, string attributeKey )
        {
            if ( element != null && element.Value.IsNotNullOrWhitespace() )
            {
                group.Attributes.Add( new GroupAttributeValue { GroupId = group.Id, AttributeKey = attributeKey, AttributeValue = element.Value } );
            }
        }

        /// <summary>
        /// Processes the boolean attribute.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="element">The element.</param>
        /// <param name="attributeKey">The attribute key.</param>
        private static void ProcessBooleanAttribute( Group group, XElement element, string attributeKey )
        {
            if ( element != null && element.Value.IsNotNullOrWhitespace() )
            {
                group.Attributes.Add( new GroupAttributeValue { GroupId = group.Id, AttributeKey = attributeKey, AttributeValue = element.Value.AsBoolean().ToString() } );
            }
        }

        /// <summary>
        /// Processes the datetime attribute.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="element">The element.</param>
        /// <param name="attributeKey">The attribute key.</param>
        private static void ProcessDatetimeAttribute( Group group, XElement element, string attributeKey )
        {
            if ( element != null && element.Value.AsDateTime().HasValue )
            {
                group.Attributes.Add( new GroupAttributeValue { GroupId = group.Id, AttributeKey = attributeKey, AttributeValue = element.Value.AsDateTime().ToString() } );
            }
        }
    }

    /// <summary>
    /// Thrown when a CCB department or director id cannot be turned into a unique director group id.
    /// The group export stops instead of writing colliding or zero ids.
    /// </summary>
    public class DirectorGroupIdOutOfRangeException : Exception
    {
        public DirectorGroupIdOutOfRangeException( string message ) : base( message )
        {
        }
    }
}