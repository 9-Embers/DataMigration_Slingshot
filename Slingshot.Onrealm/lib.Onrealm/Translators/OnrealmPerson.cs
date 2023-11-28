using lib.Onrealm.Contracts;
using Slingshot.Core.Model;
using Slingshot.Core;

namespace lib.Onrealm.Translators;
public static class OnrealmPerson
{
    public static Person? Translate( Individual individual, int personId, int campusId, int familyId)
    {
        Person person = new();
        List<string> notes = new();


        if(individual.IndividualId != null )
        {
            person.Id = personId;

            person.FirstName = individual.FirstName;
            person.NickName = individual.PreferredName ?? individual.FirstName;
            person.LastName = individual.LastName;
            person.Email = individual.PrimaryEmail;
            person.MiddleName = individual.MiddleName;
            person.Suffix = individual.Suffix;
            person.Salutation = individual.Title;
            person.Birthdate = individual.DateofBirth.AsDateTime();
            person.AnniversaryDate = individual.AnniversaryDate.AsDateTime();
            person.CreatedDateTime = individual.AddedWhen;

            if(individual.DoNotEmail == "Yes")
            {
                person.EmailPreference = EmailPreference.DoNotEmail;
            }

            //Gender
            if(individual.Gender == "Male")
            {
                person.Gender = Gender.Male;
            }
            else if(individual.Gender == "Female")
            {
                person.Gender = Gender.Female;
            }
            else
            {
                person.Gender = Gender.Unknown;
            }

            //Marital Status
            switch(individual.MaritalStatus)
            {
                case "Single":
                    person.MaritalStatus = MaritalStatus.Single;
                    break;
                case "Married":
                    person.MaritalStatus = MaritalStatus.Married;
                    break;
                case "Divorced":
                    person.MaritalStatus = MaritalStatus.Divorced;
                    break;
                default:
                    person.MaritalStatus = MaritalStatus.Unknown;
                    if(individual.MaritalStatus.IsNotNullOrWhitespace())
                    {
                        notes.Add("marital status: " + individual.MaritalStatus);
                    }
                    break;
            }

            var connectionStatus = individual.MemberStatus;
            if (connectionStatus.IsNullOrWhiteSpace())
            {
                // default to attendee - gotta provide something... 
                connectionStatus = "Attendee";
            }
            person.ConnectionStatus = connectionStatus;

            var individualStatus = connectionStatus != "Deceased" ? individual.IndividualStatus : "Deceased";
            switch(individualStatus)
            {
                case "Active":
                    person.RecordStatus = RecordStatus.Active;
                    break;
                case "Inactive":
                    person.RecordStatus = RecordStatus.Inactive;
                    break;
                case "Deceased":
                    person.RecordStatus = RecordStatus.Inactive;
                    person.InactiveReason = "Deceased";
                    break;
                default:
                    break;
            }

            if(individual.DateMarkedDeceased.IsNotNullOrWhitespace())
            {
                notes.Add($"Date Marked Deceased: {individual.DateMarkedDeceased}" );
            }

            if(individual.DateofBirth.IsNotNullOrWhitespace())
            {
                notes.Add($"Date of Birth: {individual.DateofBirth}" );
            }

            if(individual.PrimaryPhoneNumber.IsNotNullOrWhitespace())
            {
                person.PhoneNumbers.Add(new PersonPhone
                {
                    PersonId = personId,
                    PhoneType = "Home",
                    PhoneNumber = individual.PrimaryPhoneNumber.Substring(0, Math.Min(20, individual.PrimaryPhoneNumber.Length)),
                    IsUnlisted = individual.HomePhonePrivacy.IsNotNullOrWhitespace() && individual.HomePhonePrivacy != "Anyone"
                });
            }

            if(individual.MobilePhoneNumber.IsNotNullOrWhitespace())
            {
                person.PhoneNumbers.Add(new PersonPhone
                {
                    PersonId = personId,
                    PhoneType = "Mobile",
                    PhoneNumber = individual.MobilePhoneNumber.Substring(0, Math.Min(20, individual.MobilePhoneNumber.Length)),
                    IsUnlisted = individual.MobilePhonePrivacy.IsNotNullOrWhitespace() && individual.MobilePhonePrivacy != "Anyone",
                    IsMessagingEnabled = true
                });
            }

            if(individual.WorkPhoneNumber.IsNotNullOrWhitespace())
            {
                person.PhoneNumbers.Add(new PersonPhone
                {
                    PersonId = personId,
                    PhoneType = "Work",
                    PhoneNumber = individual.WorkPhoneNumber.Substring(0, Math.Min(20, individual.WorkPhoneNumber.Length)),
                    IsUnlisted = individual.WorkPhonePrivacy.IsNotNullOrWhitespace() && individual.WorkPhonePrivacy != "Anyone",
                });
            }

            if(individual.OtherPhoneNumber.IsNotNullOrWhitespace())
            {
                person.PhoneNumbers.Add(new PersonPhone
                {
                    PersonId = personId,
                    PhoneType = "Contact",
                    PhoneNumber = individual.OtherPhoneNumber.Substring(0, Math.Min(20, individual.OtherPhoneNumber.Length)),
                    IsUnlisted = individual.OtherPhonePrivacy.IsNotNullOrWhitespace() && individual.OtherPhonePrivacy != "Anyone",
                });
            }

            if(individual.MailingAddress1.IsNotNullOrWhitespace())
            {
                PersonAddress address = new()
                {
                    AddressType = AddressType.Home,
                    PersonId = personId,
                    Street1 = individual.MailingAddress1,
                    Street2 = individual.MailingAddress2,
                    City = individual.MailingCity,
                    State = individual.MailingRegion,
                    PostalCode = individual.MailingPostalCode,
                    Country = individual.MailingCountry,
                    IsMailing = true,
                };

                if (address.City.IsNotNullOrWhitespace() && address.PostalCode.IsNotNullOrWhitespace())
                {
                    person.Addresses.Add(address);
                }
            }

            person.FamilyId = familyId;

            if (person.Birthdate.HasValue)
            {
                var age = GetAge(person.Birthdate, individual.DateMarkedDeceased.AsDateTime());
                person.FamilyRole = age.HasValue && age.Value < 18 ? FamilyRole.Child : FamilyRole.Adult;
            }

            Campus campus = new();
            person.Campus = campus;

            if(individual.Campus.IsNotNullOrWhitespace())
            {
                campus.CampusId = campusId;
                campus.CampusName = individual.Campus;
            }

            ProcessBooleanAttribute(person, individual.BaptismCompleted, "BaptismCompleted");
            ProcessDateAttribute(person, individual.BaptismDate, "BaptismDate");
            ProcessStringAttribute(person, individual.BaptismNotes, "BaptismNotes");
            ProcessBooleanAttribute(person, individual.RootedCompleted, "RootedCompleted");
            ProcessDateAttribute(person, individual.RootedDate, "RootedDate");
            ProcessStringAttribute(person, individual.RootedNotes, "RootedNotes");
            ProcessBooleanAttribute(person, individual.SalvationRededicatedLifeCompleted, "SalvationRededicatedLifeCompleted");
            ProcessDateAttribute(person, individual.SalvationRededicatedLifeDate, "SalvationRededicatedLifeDate");
            ProcessStringAttribute(person, individual.SalvationRededicatedLifeNotes, "SalvationRededicatedLifeNotes");
            ProcessBooleanAttribute(person, individual.BibleMilestoneCompleted, "BibleMilestoneCompleted");
            ProcessDateAttribute(person, individual.BibleMilestoneDate, "BibleMilestoneDate");
            ProcessStringAttribute(person, individual.BibleMilestoneNotes, "BibleMilestoneNotes");
            ProcessBooleanAttribute(person, individual.MarriageCompleted, "MarriageCompleted");
            ProcessDateAttribute(person, individual.MarriageDate, "MarriageDate");
            ProcessStringAttribute(person, individual.MarriageNotes, "MarriageNotes");
            ProcessBooleanAttribute(person, individual.ChildDedicationCompleted, "ChildDedicationCompleted");
            ProcessDateAttribute(person, individual.ChildDedicationDate, "ChildDedicationDate");
            ProcessStringAttribute(person, individual.ChildDedicationNotes, "ChildDedicationNotes");
            ProcessDateAttribute(person, individual.GiftDatefirst, "GiftDatefirst");
            ProcessDateAttribute(person, individual.GiftDatelast, "GiftDateLast");
            ProcessBooleanAttribute(person, individual.SharedGiving, "SharedGiving");
            ProcessStringAttribute(person, individual.GivingNumber, "GivingNumber");
            ProcessStringAttribute(person, individual.AllergiesMedicalConcerns, "AllergiesMedicalConcerns");
            ProcessStringAttribute(person, individual.HSGraduationYear, "HSGraduationYear");
            ProcessStringAttribute(person, individual.InsuranceCarrier, "InsuranceCarrier");
            ProcessStringAttribute(person, individual.InsurancePolicyNumber, "InsurancePolicyNumber");
            ProcessDateAttribute(person, individual.LastBackgroundCheckDate, "LastBackgroundCheckDate");
            ProcessStringAttribute(person, individual.LastBackgroundCheckPackageName, "LastBackgroundCheckPackageName");
            ProcessStringAttribute(person, individual.LastBackgroundCheckStatus, "LastBackgroundCheckStatus");
            ProcessStringAttribute(person, individual.MaidenName, "MaidenName");
            ProcessBooleanAttribute(person, individual.MandatedReporterCompletion, "MandatedReporterCompletion");
            ProcessDateAttribute(person, individual.MandatedReporterExpiration, "MandatedReporterExpiration");
            ProcessDateAttribute(person, individual.RootedGraduationDate, "RootedGraduationDate");
            ProcessStringAttribute(person, individual.NoRegretsGoldGraduate, "NoRegretsGoldGraduate");
            ProcessStringAttribute(person, individual.NoRegretsSilvergraduate, "NoRegretsSilverGraduate");

            if(notes.Count > 0)
            {
                person.Note = string.Join(",", notes);
            }

            return person;
        }

        return null;
    }

    public static void ProcessBooleanAttribute( Person person, string value, string attributeKey)
    {
        if(value.IsNotNullOrWhitespace())
        {
            person.Attributes.Add(new PersonAttributeValue
            {
                PersonId = person.Id,
                AttributeKey = attributeKey,
                AttributeValue = value.Trim().AsBoolean().ToString()
            });
        }
    }

    public static void ProcessStringAttribute( Person person, string value, string attributeKey)
    {
        if(value.IsNotNullOrWhitespace())
        {
            person.Attributes.Add(new PersonAttributeValue
            {
                PersonId = person.Id,
                AttributeKey = attributeKey,
                AttributeValue = value.Trim()
            });
        }
    }

    public static void ProcessDateAttribute( Person person, string value, string attributeKey)
    {
        value = value?.Trim('-')?.Trim('\'') ?? string.Empty;
        if(value.IsNotNullOrWhitespace())
        {
            person.Attributes.Add(new PersonAttributeValue
            {
                PersonId = person.Id,
                AttributeKey = attributeKey,
                AttributeValue = value.Trim().AsDateTime().ToString()
            });
        }
    }

    public static int? GetAge(DateTime? birthDate, DateTime? deceasedDate)
    {
        if (birthDate.HasValue && birthDate.Value.Year != DateTime.MinValue.Year)
        {
            DateTime asOfDate = deceasedDate.HasValue ? deceasedDate.Value : DateTime.Today;
            int age = asOfDate.Year - birthDate.Value.Year;
            if (birthDate.Value > asOfDate.AddYears(-age))
            {
                // their birthdate is after today's date, so they aren't a year older yet
                age--;
            }

            return age;
        }

        return null;
    }
}
