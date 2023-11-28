using lib.Onrealm.Contracts;
using Slingshot.Core.Model;
using Slingshot.Core;

namespace lib.Onrealm.Translators;
public static class OnrealmFinancialBatch
{
    public static FinancialBatch Translate(Batch inputBatch, int batchId, int modifiedPersonId, int campusId)
    {
        FinancialBatch batch = new();
        List<string> descriptions = new();

        if(inputBatch.Id is not null && inputBatch.Id.IsNotNullOrWhitespace())
        {
            batch.Id = batchId;
            batch.StartDate = inputBatch?.BatchDate?.AsDateTime() ?? inputBatch?.DateCreated?.AsDateTime() ?? DateTime.MinValue;
            batch.CampusId = campusId;

            if(inputBatch.DatePosted is not null && inputBatch?.DatePosted?.ToString()?.AsDateTime() is not null)
            {
                batch.EndDate = inputBatch?.DatePosted?.ToString()?.AsDateTime();
            }

            batch.Name = inputBatch.BatchNumber?.ToString() ?? inputBatch.Id;
            batch.CreatedDateTime = inputBatch?.DateCreated?.AsDateTime() ?? DateTime.MinValue;
            batch.ModifiedDateTime = inputBatch?.DateModified?.AsDateTime() ?? DateTime.MinValue;
            batch.ModifiedByPersonId = modifiedPersonId;

            if(inputBatch.Description is not null && inputBatch.Description.IsNotNullOrWhitespace())
            {
                descriptions.Add(inputBatch.Description);
            }

            if (inputBatch.MintBatchRef is not null && inputBatch.MintBatchRef.IsNotNullOrWhitespace())
            {
                descriptions.Add($"Mint Batch Ref: {inputBatch.MintBatchRef}");
            }

            if (inputBatch.Type.HasValue)
            {
                descriptions.Add($"Type: {inputBatch.Type}");
            }

            if (inputBatch.SubType.HasValue)
            {
                descriptions.Add($"SubType: {inputBatch.SubType}");
            }

            if (inputBatch.SiteId is not null && inputBatch.SiteId.IsNotNullOrWhitespace())
            {
                descriptions.Add($"SiteId: {inputBatch.SiteId}");
            }

            if(inputBatch.ExpectedAmount is not null && inputBatch.ExpectedAmount.ToString().IsNotNullOrWhitespace())
            {
                descriptions.Add($"Expected Amount: {inputBatch.ExpectedAmount.ToString()}");
            }

            if (inputBatch.DefaultFundId is not null && inputBatch.DefaultFundId.ToString().IsNotNullOrWhitespace())
            {
                descriptions.Add($"Default Fund Id: {inputBatch.DefaultFundId}");
            }

            if (inputBatch.DefaultOnlinePaymentSourceTypeId is not null && inputBatch.DefaultOnlinePaymentSourceTypeId.ToString().IsNotNullOrWhitespace())
            {
                descriptions.Add($"Default Online Payment Source Type Id: {inputBatch.DefaultOnlinePaymentSourceTypeId}");
            }

            if (inputBatch.DefaultPaymentType is not null && inputBatch.DefaultPaymentType.ToString().IsNotNullOrWhitespace())
            {
                descriptions.Add($"Default Payment Type: {inputBatch.DefaultPaymentType}");
            }

            if (inputBatch.DepositAccountId is not null && inputBatch.DepositAccountId.IsNotNullOrWhitespace())
            {
                descriptions.Add($"Deposit Account Id: {inputBatch.DepositAccountId}");
            }

            if (inputBatch.TotalAmount.HasValue)
            {
                descriptions.Add($"Total Amount: {inputBatch.TotalAmount}");
            }

            if (inputBatch.TotalGiftCount.HasValue)
            {
                descriptions.Add($"Total Gift Count: {inputBatch.TotalGiftCount}");
            }

            if (inputBatch.GlDepositReference is not null && inputBatch.GlDepositReference.IsNotNullOrWhitespace())
            {
                descriptions.Add($"GL Deposit Reference: {inputBatch.GlDepositReference}");
            }

            if(batch.EndDate.HasValue)
            {
                batch.Status = BatchStatus.Closed;
            }
            else
            {
                batch.Status = BatchStatus.Open;
            }

        }

        return batch;
    }
}
