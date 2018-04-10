using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        private static int MaxWordWrapLineLength = 20;
        private static int MaxWordWrapColumnWidth = MaxWordWrapLineLength + 5;
        private static int SkipResultsDefault = 0;
        private static int TakeDefault = 10;

        [Required]
        [Argument(0, "query", Description = "The search terms used to find packages")]
        public string Query { get; set; }

        [Option("--include-prerelease", "Include prerelease packages", CommandOptionType.NoValue)]
        public bool IncludePrerelease { get; set; }

        [Option("-s|--skip", "Number of results to skip. (Default: 0)", CommandOptionType.SingleValue)]
        public int Skip { get; set; } = SkipResultsDefault;

        [Option("-t|--take", "Number of results to display. (Default: 10)", CommandOptionType.SingleValue)]
        public int Take { get; set; } = TakeDefault;

        private async Task OnExecuteAsync()
        {
            var httpClient = new HttpClient();
            var url = $"https://api-v2v3search-0.nuget.org/query?q={Query}&prerelease={IncludePrerelease}&skip={Skip}&take={Take}";
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
            PrintTable(searchResponse);

            var numResultsToDisplay = Take < searchResponse.TotalHits ? Take : searchResponse.TotalHits;
            var starting = Skip + 1;
            var page = (Skip + Take) / Take;
            var ending = page * Take;
            if (ending > searchResponse.TotalHits)
                ending = searchResponse.TotalHits;
            
            Console.WriteLine($"{Skip+1} - {ending} of {searchResponse.TotalHits} results");
        }

        private void PrintTable(SearchResponse searchResponse)
        {
            var data = searchResponse.Data;

            var columnPad = "   ";
            var headerBoarder = '_';
            var rowDivider = '-';

            var items = new List<(string header, bool wordWrap, Func<Package, object> valueFunc)>
             {
                 ("Name", false, x => x.Id),
                 ("Description", true, x => x.Description),
                 ("Authors", true, x => string.Join(",", x.Authors)),
                 ("Version", false, x => x.Version),
                 ("Downloads", false, x => x.TotalDownloads.ToAbbrString()),
                 ("Verified", false, x => x.Verified ? "   *" : "")
             };

            var columns = new List<string>[items.Count];
            var headers = new string[items.Count];
            var columnWidths = new int[items.Count];
            var valueCount = 0;

            foreach (var d in data)
            {
                int index = 0;
                foreach (var act in items)
                {
                    headers[index] = act.header;
                    var columnValue = act.valueFunc(d)?.ToString() ?? "(null)";
                    columnValue = columnValue.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");

                    if (columns[index] == null)
                    {
                        columns[index] = new List<string>();
                    }

                    columns[index++].Add(columnValue);
                }
                valueCount++;
            }

            if (valueCount == 0)
            {
                Console.WriteLine("No results founds");
                return;
            }

            for (var i = 0; i < columns.Length; i++)
            {
                var wordWrap = items[i].wordWrap;

                var width = Math.Max(columns[i].Max(x => x.Length), headers[i].Length);
                if (wordWrap)
                {
                    width = Math.Min(width, MaxWordWrapColumnWidth);
                }

                columnWidths[i] = width;
            }

            int headerWidth = columnWidths.Sum() + columnPad.Length * (items.Count - 1);

            // Determine final layout, word wrapping etc
            var rows = new List<string[]>();
            for (var i = 0; i < valueCount; i++)
            {
                var rowsToAdd = new List<string[]>();
                var newRow = new string[columns.Length];
                rowsToAdd.Add(newRow);
                for (var j = 0; j < columns.Length; j++)
                {
                    var value = columns[j][i];
                    if (value.Length <= columnWidths[j])
                    {
                        newRow[j] = value;
                        continue;
                    }

                    // Word wrap it
                    var wordWrapRows = GetWordWrapRows(value);
                    newRow[j] = wordWrapRows[0];

                    for (var x = 1; x < wordWrapRows.Count; x++)
                    {
                        if (rowsToAdd.Count < x + 1)
                        {
                            rowsToAdd.Add(new string[columns.Length]);
                        }

                        rowsToAdd[x][j] = wordWrapRows[x];
                    }
                }

                rows.AddRange(rowsToAdd);
            }

            // Print output

            //Print headers
            for (var i = 0; i < headers.Length; i++)
            {
                Console.Write(headers[i].PadRight(columnWidths[i]));

                if (i < headers.Length - 1)
                    Console.Write(columnPad);
            }

            Console.WriteLine();

            // Print header border
            Console.WriteLine("".PadRight(headerWidth, headerBoarder));

            // Print rows
            for (var i = 0; i < rows.Count; i++)
            {
                int j = 0;
                for (; j < columns.Length; j++)
                {
                    var value = rows[i][j] ?? "";

                    if (j == 0 && i > 0 && !string.IsNullOrEmpty(value))
                    {
                        // We found a new package. Print divider
                        Console.WriteLine("".PadRight(headerWidth, rowDivider));

                    }

                    Console.Write(value.PadRight(columnWidths[j]));

                    if (j < columnWidths.Length - 1)
                    {
                        Console.Write(columnPad);
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }

        private static List<string> GetWordWrapRows(string value)
        {
            var words = value.Split(' ');
            var rows = new List<string>();
            var line = "";
            for (var i = 0; i < words.Length; i++)
            {
                var word = words[i].Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
                if ((line + word).Length > MaxWordWrapLineLength)
                {
                    rows.Add(line);
                    line = "";
                }

                line += $"{word} ";
            }

            rows.Add(line);

            return rows;
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