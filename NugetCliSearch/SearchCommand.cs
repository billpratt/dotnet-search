using NugetCliSearch.Services;

namespace NugetCliSearch;

[Command(
    Name = "dotnet nuget-search",
    FullName = "dotnet-nuget-search",
    Description = "Search for Nuget packages"
)]
[HelpOption]
[VersionOptionFromMember(MemberName = nameof(GetVersion))]
internal class SearchCommand
{
    private readonly HttpClient _httpClient;
    private readonly NugetIndexReader _nugetIndexReader;
    
    private const int SkipResultsDefault = 0;
    private const int TakeDefault = 10;

    [Argument(0, "query", Description = "The search terms to used to filter packages")]
    public string Query { get; set; }

    [Option("-i|--include-prerelease", "Include prerelease packages", CommandOptionType.NoValue)]
    public bool IncludePrerelease { get; set; } = false;

    [Option("-s|--skip", "The number of results to skip, for pagination", CommandOptionType.SingleValue)]
    public int Skip { get; set; } = SkipResultsDefault;

    [Option("-t|--take", "The number of results to return, for pagination", CommandOptionType.SingleValue)]
    public int Take { get; set; } = TakeDefault;

    [Option("-p|--package-type", "The package type to use to filter packages", CommandOptionType.SingleValue)]
    [AllowedValues("dependency", "dotnettool", "dotnetclitool", "template", "all")]
    public string PackageType { get; set; } = "all";

    [Option("-l|--list", "Print results as full detail list", CommandOptionType.NoValue)]
    public bool List { get; set; } = false;

    [Option("-v|--verbose", "Prints all messages to standard output", CommandOptionType.NoValue)]
    public bool Verbose { get; set; } = false;

    [Option("-r|--repository", "The repository to use for searching", CommandOptionType.SingleValue)]
    public string Repository { get; set; }

    public SearchCommand(HttpClient httpClient, NugetIndexReader nugetIndexReader)
    {
        _httpClient = httpClient;
        _nugetIndexReader = nugetIndexReader;
    }

    public async Task OnExecuteAsync()
    {
        var queryUrl = await GetQueryUrl();

        var url =
            $"{queryUrl}?q={Query}&semVerLevel=2.0.0&prerelease={IncludePrerelease}&skip={Skip}&take={Take}&packageType={(PackageType.ToLowerInvariant() == "all" ? "" : PackageType)}";
        try
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var searchResponse = await response.Content.ReadAsAsync<SearchResult>();

            if (!searchResponse.Data.Any())
            {
                Console.WriteLine($"No results found for \"{Query}\"");
                return;
            }

            if (List)
                SearchResultsPrinter.PrintAsList(searchResponse);
            else
                SearchResultsPrinter.PrintAsTable(searchResponse);

            PrintFooter(searchResponse);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error {e.Message}");
        }
    }

    private async Task<string> GetQueryUrl()
    {
        if (string.IsNullOrWhiteSpace(Repository))
            Repository = await _nugetIndexReader.GetIndexUrl();

        var queryUrl = Repository.EndsWith("/query") ? Repository : Repository.TrimEnd('/') + "/query";
        return queryUrl;
    }

    private void PrintFooter(SearchResult searchResponse)
    {
        var page = (Skip + Take) / Take;
        var ending = page * Take;
        if (ending > searchResponse.TotalHits)
            ending = searchResponse.TotalHits;

        Console.WriteLine($"{ending} of {searchResponse.TotalHits} results");
    }

    private static string GetVersion()
        => typeof(SearchCommand).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
}