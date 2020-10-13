using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using DotNetSearch.Extensions;
using McMaster.Extensions.CommandLineUtils;
using System.IO;
using System.Text;
using System.Xml;
using Newtonsoft.Json;

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

        [Option("-f|--format", "Output format. (Default: table, possible values are table, json, xml)", CommandOptionType.SingleValue)]
        public string Format { get; set; }

        [Option("-u|--utf8", "Output UTF-8 instead of system default encoding.(no bom)", CommandOptionType.NoValue)]
        public bool IsUtf8 { get; set; }

        [Option("-o|--output", "Output file path. (Default: stdout)", CommandOptionType.SingleValue)]
        public string OutputPath { get; set; }

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

        Stream OpenOutputStream(bool overwrite)
        {
            if (!string.IsNullOrEmpty(OutputPath))
            {
                if(overwrite)
                {
                    return File.Create(OutputPath);
                }
                else
                {
                    var ret = File.OpenWrite(OutputPath);
                    ret.Seek(0, SeekOrigin.End);
                    return ret;
                }
            }
            else
            {
                return Console.OpenStandardOutput(4096);
            }
        }

        Encoding GetOutputEncoding()
        {
            if (IsUtf8)
            {
                return new UTF8Encoding(false);
            }
            else
            {
                return Encoding.Default;
            }
        }

        private void PrintResults(SearchResponse searchResponse)
        {
            var fmt = !string.IsNullOrEmpty(Format) ? Format : "table";
            switch (fmt)
            {
                case "json":
                    PrintJson(searchResponse);
                    break;
                case "xml":
                    PrintXml(searchResponse);
                    break;
                case "table":
                default:
                    {
                        PrintTable(searchResponse);

                        var numResultsToDisplay = Take < searchResponse.TotalHits ? Take : searchResponse.TotalHits;
                        var starting = Skip + 1;
                        var page = (Skip + Take) / Take;
                        var ending = page * Take;
                        if (ending > searchResponse.TotalHits)
                            ending = searchResponse.TotalHits;

                        using (var stm = OpenOutputStream(false))
                        using (var tw = new StreamWriter(stm, GetOutputEncoding()))
                        {
                            tw.WriteLine($"{Skip + 1} - {ending} of {searchResponse.TotalHits} results");
                        }
                    }
                    break;
            }
        }

        static int GetActualResults(int skip, int take, int totalHits)
        {
            if(skip >= totalHits)
            {
                return 0;
            }
            if(skip + take > totalHits)
            {
                return totalHits - skip;
            }
            else
            {
                return take;
            }
        }

        private void PrintXml(SearchResponse searchResponse)
        {
            var xwsetting = new XmlWriterSettings();
            xwsetting.Indent = true;
            using (var stm = OpenOutputStream(true))
            using (var tw = new StreamWriter(stm, GetOutputEncoding()))
            using (var xw = XmlWriter.Create(tw, xwsetting))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("results");
                xw.WriteStartElement("packages");
                xw.WriteAttributeString("total", searchResponse.TotalHits.ToString());
                xw.WriteAttributeString("skip", Skip.ToString());
                xw.WriteAttributeString("take", GetActualResults(Skip, Take, searchResponse.TotalHits).ToString());
                foreach (var pkg in searchResponse.Data)
                {
                    xw.WriteStartElement("package");
                    xw.WriteElementString("ApiId", pkg.ApiId);
                    xw.WriteStartElement("Authors");
                    foreach (var author in pkg.Authors)
                    {
                        xw.WriteElementString("Author", author);
                    }
                    xw.WriteEndElement(); // Authors
                    xw.WriteElementString("Descritpion", pkg.Description);
                    xw.WriteElementString("IconUrl", pkg.IconUrl);
                    xw.WriteElementString("Id", pkg.Id);
                    xw.WriteElementString("LicenseUrl", pkg.LicenseUrl);
                    xw.WriteElementString("ProjectUrl", pkg.ProjectUrl);
                    xw.WriteElementString("Registration", pkg.Registration);
                    xw.WriteElementString("Summary", pkg.Summary);
                    xw.WriteStartElement("Tags");
                    foreach (var tag in pkg.Tags)
                    {
                        xw.WriteElementString("Tag", tag);
                    }
                    xw.WriteEndElement(); // Tags
                    xw.WriteElementString("Title", pkg.Title);
                    xw.WriteElementString("TotalDownloads", pkg.TotalDownloads.ToString());
                    xw.WriteElementString("Type", pkg.Type);
                    xw.WriteElementString("Verified", pkg.Verified.ToString());
                    xw.WriteElementString("LatestVersion", pkg.Version);
                    xw.WriteStartElement("Versions");
                    foreach (var v in pkg.Versions)
                    {
                        xw.WriteStartElement("Version");
                        xw.WriteElementString("Version", v.Version);
                        xw.WriteElementString("Id", v.Id);
                        xw.WriteElementString("Downloads", v.Downloads.ToString());
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement(); // Versions
                    xw.WriteEndElement(); // package
                }
                xw.WriteEndElement();// packages
                xw.WriteEndElement();// results
                xw.WriteEndDocument();
            }
        }

        private void PrintJson(SearchResponse searchResponse)
        {
            using (var stm = OpenOutputStream(true))
            using (var tw = new StreamWriter(stm, GetOutputEncoding()))
            using (var jw = new JsonTextWriter(tw))
            {
                jw.Formatting = Newtonsoft.Json.Formatting.Indented;
                jw.WriteStartObject();
                jw.WritePropertyName("total");
                jw.WriteValue(searchResponse.TotalHits);
                jw.WritePropertyName("skip");
                jw.WriteValue(Skip);
                jw.WritePropertyName("take");
                jw.WriteValue(GetActualResults(Skip, Take, searchResponse.TotalHits));
                jw.WritePropertyName("packages");
                jw.WriteStartArray();
                var serializer = new JsonSerializer();
                foreach (var pkg in searchResponse.Data)
                {
                    serializer.Serialize(jw, pkg);
                }
                jw.WriteEndArray();
                jw.WriteEndObject(); // root
            }
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

            using (var stm = OpenOutputStream(true))
            using (var tw = new StreamWriter(stm, GetOutputEncoding()))
            {
                //Print headers
                for (var i = 0; i < headers.Length; i++)
                {
                    tw.Write(headers[i].PadRight(columnWidths[i]));

                    if (i < headers.Length - 1)
                        tw.Write(columnPad);
                }

                tw.WriteLine();

                // Print header border
                tw.WriteLine("".PadRight(headerWidth, headerBoarder));

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
                            tw.WriteLine("".PadRight(headerWidth, rowDivider));

                        }

                        tw.Write(value.PadRight(columnWidths[j]));

                        if (j < columnWidths.Length - 1)
                        {
                            tw.Write(columnPad);
                        }
                    }

                    tw.WriteLine();
                }

                tw.WriteLine();
            }
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