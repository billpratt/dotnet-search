using DotNetSearch.Services;

namespace DotNetSearch;

[Command(
    Name = "dotnet search",
    FullName = "dotnet-search",
    Description = "Search for Nuget packages"
)]
[HelpOption]
[VersionOptionFromMember(MemberName = nameof(GetVersion))]
internal class SearchCommand
{
    private readonly HttpClient _httpClient;
    private readonly NugetIndexReader _nugetIndexReader;
    private const int SKIP_RESULTS_DEFAULT = 0;
    private const int TAKE_DEFAULT = 10;

    [Required]
    [Argument(0, "query", Description = "The search terms to used to filter packages")]
    public string Query { get; set; }

    [Option("--include-prerelease", "Include prerelease packages", CommandOptionType.NoValue)]
    public bool IncludePrerelease { get; set; }

    [Option("-s|--skip", "The number of results to skip, for pagination (Default: 0)", CommandOptionType.SingleValue)]
    public int Skip { get; set; } = SKIP_RESULTS_DEFAULT;

    [Option("-t|--take", "The number of results to return, for pagination (Default: 10)", CommandOptionType.SingleValue)]
    public int Take { get; set; } = TAKE_DEFAULT;

    [Option("-p|--package-type", "The package type to use to filter packages", CommandOptionType.SingleValue)]
    public string PackageType { get; set; } = "";

    [Option("-l|--list", "Print results as full detail list", CommandOptionType.NoValue)]
    public bool List { get; set; }

    [Option("-v|--verbose", "Prints all messages to standard output", CommandOptionType.NoValue)]
    public bool Verbose { get; set; }

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
            $"{queryUrl}?q={Query}&prerelease={IncludePrerelease}&skip={Skip}&take={Take}&packageType={PackageType}";
        try
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var searchResponse = await response.Content.ReadAsAsync<SearchResult>();

            if (!searchResponse.Data.Any())
            {
                Console.WriteLine($"No results found for {Query}");
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