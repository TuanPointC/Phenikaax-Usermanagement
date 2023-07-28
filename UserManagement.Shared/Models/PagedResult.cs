using System.Text.Json.Serialization;

namespace UserManagement.Shared.Models;

public class PagedResult<T>
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public List<T> Results { get; set; } = null!;

    public PagedResult(PagedList<T> pagedList)
    {
        CurrentPage = pagedList.CurrentPage;
        TotalPages = pagedList.TotalPages;
        PageSize = pagedList.PageSize;
        TotalCount = pagedList.TotalCount;
        Results = pagedList.ToList();
    }
    
    [JsonConstructor]
    public PagedResult()
    {
        
    }
}
