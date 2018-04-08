using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using DotNetSearch.Extensions;
using McMaster.Extensions.CommandLineUtils;

namespace DotNetSearch
{
    [Command(
        Name = "dotnet search",
        FullName = "dotnet-search",
        Description = "Search for Nuget packages"
    )]
    [HelpOption]
    [VersionOptionFromMember(MemberName = nameof(GetVersion))]
    class SearchCommand
    {
        private static int MaxOutputStringLength = 20;
        private static int TakeDefault = 10;

        [Required]
        [Argument(0, "query", Description = "The search terms used to find packages")]
        public string Query { get; set; }

        [Option("--include-prerelease", "Include prerelease packages", CommandOptionType.NoValue)]
        public bool IncludePrerelease { get; set; }

        [Option("-n|--num-results", "Number of results to display. (Default: 10)", CommandOptionType.SingleValue)]
        public int NumberOfResults { get; set; } = TakeDefault;

        [Option("-o|--output", "One of: table | full", CommandOptionType.SingleValue)]
        public string Output { get; set; } = "";
        
        private async Task OnExecuteAsync()
        {
            var httpClient = new HttpClient();
            var url = $"https://api-v2v3search-0.nuget.org/query?q={Query}&prerelease={IncludePrerelease}&take={NumberOfResults}";
            var response = await httpClient.GetAsync(url);
            var searchResponse = await response.Content.ReadAsAsync<SearchResponse>();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"No results found for {Query}");
                return;
            }

            PrintResults(searchResponse);
        }

        private void PrintResults(SearchResponse searchResponse)
        {
            switch(Output)
            {
                case var o when (Output.Equals("full", StringComparison.InvariantCultureIgnoreCase)):
                    PrintFullDetails(searchResponse);
                    break;
                default:
                    PrintTable(searchResponse);
                    break;

            }

            var numResultsToDisplay = NumberOfResults < searchResponse.TotalHits ? NumberOfResults : searchResponse.TotalHits;
            Console.WriteLine($"{numResultsToDisplay} of {searchResponse.TotalHits} results found");
        }

        private void PrintTable(SearchResponse searchResponse)
        {
            TableFormatter.Print(searchResponse.Data, "No results found", '_', new Dictionary<string, Func<Package, object>>
            {
                { "Name", x => x.Id},
                { "Description", x => x.Description.Length > MaxOutputStringLength
                                        ? $"{x.Description.Substring(0, MaxOutputStringLength-1)}..."
                                        : x.Description },
                { "Authors", x => {
                    var joined = string.Join(",", x.Authors);
                    return joined.Length > MaxOutputStringLength
                            ? $"{joined.Substring(0, MaxOutputStringLength - 1)}..."
                            : joined;
                }},
                { "Version", x => x.Version},
                { "Downloads", x => x.TotalDownloads.ToAbbrString() },
                { "Verified", x => x.Verified ? "    *" : "" }
            });
        }

        private void PrintFullDetails(SearchResponse searchResponse)
        {
            foreach (var data in searchResponse.Data)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(data.Id);
                Console.ResetColor();
                Console.WriteLine($" by {string.Join(",", data.Authors)}\tv{data.Version}");
                Console.WriteLine($"{data.Description}{Environment.NewLine}");
                Console.WriteLine($"Tags: {string.Join(',', data.Tags)}");
                Console.WriteLine($"Downloads: {data.TotalDownloads.ToAbbrString()}");
                Console.WriteLine($"{Environment.NewLine}----------------------{Environment.NewLine}");
            }
        }

        private static string GetVersion()
                    => typeof(SearchCommand).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}