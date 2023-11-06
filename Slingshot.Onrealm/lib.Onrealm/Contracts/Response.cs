namespace lib.Onrealm.Contracts;

public class Response<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int? TotalCount { get; set; }
    public decimal? TotalAmount { get; set; } //How to know your api is trash.
    public int SharedTotalCount { get; set; }
    public bool? HasMore { get; set; }
    public string? SortName { get; set; }
    public bool? SortAscending { get; set; }
    public int? GridPageSize { get; set; }
}
