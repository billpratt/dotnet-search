namespace NugetCliSearch.Models;

public class SearchResult
{
    public int TotalHits { get; set; }
    public IEnumerable<Package> Data { get; set; }
}