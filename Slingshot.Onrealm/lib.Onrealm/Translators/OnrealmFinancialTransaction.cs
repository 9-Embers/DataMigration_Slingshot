using lib.Onrealm.Contracts;
using Slingshot.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.Onrealm.Translators;
public static class OnrealmFinancialTransaction
{
    public static FinancialTransaction Translate (ManualContribution contribution, int id)
    {
        FinancialTransaction transaction = new();

        transaction.Id = id;
        //transaction.BatchId = contribution.

        return transaction;
    }

    public static FinancialTransaction Translate(OnlineContribution contribution)
    {
        FinancialTransaction transaction = new();

        return transaction;
    }
}
