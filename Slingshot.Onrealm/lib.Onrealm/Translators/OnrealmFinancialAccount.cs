using lib.Onrealm.Contracts;
using Slingshot.Core.Model;
using Slingshot.Core;

namespace lib.Onrealm.Translators;
public static class OnrealmFinancialAccount
{
    public static FinancialAccount Translate(Fund fund, int accountId, int campusId)
    {
        FinancialAccount account = new();

        if(fund.Id.IsNotNullOrWhitespace())
        {
            account.Id = accountId;
            account.Name = fund.Name;
            account.CampusId = campusId;
            account.IsTaxDeductible = fund.IsDeductible ?? false;

        }

        return account;
    }
}
