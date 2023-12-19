namespace DotNetSearch.Services;

public class NugetIndexReader
{
    private const string NUGET_INDEX_URL = "https://api.nuget.org/v3/index.json";
    private const string SEARCH_QUERY_SERVICE_BRANCH = "SearchQueryService/3.5.0";
    
    private readonly HttpClient _httpClient;
    
    public NugetIndexReader(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<string> GetIndexUrl()
    {
        var response = await _httpClient.GetAsync(NUGET_INDEX_URL);
        response.EnsureSuccessStatusCode();
        
        var index = await response.Content.ReadAsAsync<NugetIndex>();
        return index.Resources.First(r => r.Type == SEARCH_QUERY_SERVICE_BRANCH).Id;
    }
}