namespace lib.Onrealm.Contracts;

public class Batch
{
    public string? Id { get; set; }
    public string? SiteId { get; set; }
    public object? BatchNumber { get; set; }
    public string? MintBatchRef { get; set; }
    public string? BatchDate { get; set; }
    public int? Type { get; set; }
    public int? SubType { get; set; }
    public string? Description { get; set; }
    public object? CampusId { get; set; }
    public object? DatePosted { get; set; }
    public object? ExpectedAmount { get; set; }
    public object? DefaultFundId { get; set; }
    public object? DefaultOnlinePaymentSourceTypeId { get; set; }
    public object? DefaultPaymentType { get; set; }
    public object? LastModifiedIndividualId { get; set; }
    public string? DateCreated { get; set; }
    public string? DateModified { get; set; }
    public string? DepositAccountId { get; set; }
    public double? TotalAmount { get; set; }
    public int? TotalGiftCount { get; set; }
    public string? GlDepositReference { get; set; }
    public object? DepositAccount { get; set; }
}
