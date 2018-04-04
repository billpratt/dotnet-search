using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace DotNetSearch
{
    [Command(
        Name = "dotnet search",
        FullName = "dotnet-search",
        Description = "Search for Nuget packages"
    )]
    [HelpOption]
    //[VersionOptionFromMember(MemberName = nameof(GetVersion))]
    class SearchCommand
    {
        private static int MaxOutputStringLength = 20;
        private static int TakeDefault = 20;

        [Required]
        [Argument(0, "query", Description = "The search terms used to find packages")]
        public string Query { get; set; }

        [Option("--include-prerelease", "Include prerelease packages", CommandOptionType.NoValue)]
        public bool IncludePrerelease { get; set; }

        public int Take { get; set; } = TakeDefault;

        private async Task OnExecuteAsync()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"https://api-v2v3search-0.nuget.org/query?q={Query}&prerelease={IncludePrerelease}&take={Take}");
            var searchResponse = await response.Content.ReadAsAsync<SearchResponse>();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"No results found for {Query}");
            }

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
                { "Downloads", x => x.TotalDownloads.ToString("#,#", CultureInfo.InvariantCulture) },
                { "Verified", x => x.Verified ? "    *" : "" }
            });
        }
        private static string GetVersion()
                    => typeof(SearchCommand).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}