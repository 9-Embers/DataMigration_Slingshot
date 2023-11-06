﻿namespace lib.Onrealm.Contracts;

public class Fund
{
    public string? Id { get; set; }
    public string? SiteId { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Checking { get; set; }
    public object? CheckingNonPledge { get; set; }
    public object? CheckingPrePledge { get; set; }
    public object? CheckingPostPledge { get; set; }
    public string? Revenue { get; set; }
    public object? RevenueNonPledge { get; set; }
    public object? RevenuePrePledge { get; set; }
    public object? RevenuePostPledge { get; set; }
    public object? CheckingDescription { get; set; }
    public object? CheckingNonPledgeDescription { get; set; }
    public object? CheckingPrePledgeDescription { get; set; }
    public object? CheckingPostPledgeDescription { get; set; }
    public object? RevenueDescription { get; set; }
    public object? RevenueNonPledgeDescription { get; set; }
    public object? RevenuePrePledgeDescription { get; set; }
    public object? RevenuePostPledgeDescription { get; set; }
    public string? CampusId { get; set; }
    public string? CampusName { get; set; }
    public string? DateCreated { get; set; }
    public string? DateModified { get; set; }
    public string? DateDeactivated { get; set; }
    public bool? IsAvailableForOnlineGiving { get; set; }
    public bool? IsMemoEnabled { get; set; }
    public string? DisplayName { get; set; }
    public int? OnlineOrder { get; set; }
    public string? CodeSort { get; set; }
    public string? CheckingSort { get; set; }
    public string? RevenueSort { get; set; }
    public object? CheckingAccountLinkId { get; set; }
    public object? CheckingAccountLink { get; set; }
    public object? CheckingNonPledgeAccountLinkId { get; set; }
    public object? CheckingNonPledgeAccountLink { get; set; }
    public object? CheckingPrePledgeAccountLinkId { get; set; }
    public object? CheckingPrePledgeAccountLink { get; set; }
    public object? CheckingPostPledgeAccountLinkId { get; set; }
    public object? CheckingPostPledgeAccountLink { get; set; }
    public object? IncomeAccountLinkId { get; set; }
    public object? IncomeAccountLink { get; set; }
    public object? IncomeNonPledgeAccountLinkId { get; set; }
    public object? IncomeNonPledgeAccountLink { get; set; }
    public object? IncomePrePledgeAccountLinkId { get; set; }
    public object? IncomePrePledgeAccountLink { get; set; }
    public object? IncomePostPledgeAccountLinkId { get; set; }
    public object? IncomePostPledgeAccountLink { get; set; }
    public object? DepositAccountId { get; set; }
    public object? DepositAccountName { get; set; }
    public bool? IsDeductible { get; set; }
    public bool? IsProcessingCostFund { get; set; }
    public bool? IsPledgeAccountSet { get; set; }
    public string? Comment { get; set; }
}
