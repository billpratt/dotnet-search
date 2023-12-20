namespace NugetCliSearch.Services;

public class NugetIndexReader
{
    private const string NugetIndexUrl = "https://api.nuget.org/v3/index.json";
    private const string SearchQueryServiceBranch = "SearchQueryService/3.5.0";
    
    private readonly HttpClient _httpClient;
    
    public NugetIndexReader(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<string> GetIndexUrl()
    {
        var response = await _httpClient.GetAsync(NugetIndexUrl);
        response.EnsureSuccessStatusCode();
        
        var index = await response.Content.ReadAsAsync<NugetIndex>();
        return index.Resources.First(r => r.Type == SearchQueryServiceBranch).Id;
    }
}