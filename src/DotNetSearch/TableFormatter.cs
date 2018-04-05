// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Originally copied from: https://github.com/dotnet/templating/blob/master/src/Microsoft.TemplateEngine.Cli/TableFormatter.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetSearch
{
    internal class TableFormatter
    {
        public static void Print<T>(IEnumerable<T> items, 
                                    string noItemsMessage, 
                                    char? headerBorder, 
                                    Dictionary<string, Func<T, object>> dictionary,
                                    string columnPad = "   ", 
                                    bool noItemsShowHeader = false)
        {
            List<string>[] columns = new List<string>[dictionary.Count];

            for (int i = 0; i < dictionary.Count; ++i)
            {
                columns[i] = new List<string>();
            }

            string[] headers = new string[dictionary.Count];
            int[] columnWidths = new int[dictionary.Count];
            int valueCount = 0;

            foreach (T item in items)
            {
                int index = 0;
                foreach (KeyValuePair<string, Func<T, object>> act in dictionary)
                {
                    headers[index] = act.Key;

                    var columnValue = act.Value(item)?.ToString() ?? "(null)";
                    columnValue = columnValue.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
                    columns[index++].Add(columnValue);
                }
                ++valueCount;
            }

            if (valueCount > 0)
            {
                for (int i = 0; i < columns.Length; ++i)
                {
                    columnWidths[i] = Math.Max(columns[i].Max(x => x.Length), headers[i].Length);
                }
            }
            else
            {
                int index = 0;
                foreach (KeyValuePair<string, Func<T, object>> act in dictionary)
                {
                    headers[index] = act.Key;
                    columnWidths[index++] = act.Key.Length;
                }
            }

            int headerWidth = columnWidths.Sum() + columnPad.Length * (dictionary.Count - 1);

            // Only show headers if we have values OR flag is set to true
            if (valueCount > 0 || noItemsShowHeader)
            {
                for (int i = 0; i < headers.Length - 1; ++i)
                {
                    Reporter.Output.Write(headers[i].PadRight(columnWidths[i]));
                    Reporter.Output.Write(columnPad);
                }

                Reporter.Output.WriteLine(headers[headers.Length - 1]);
            }


            if (headerBorder.HasValue)
            {
                Reporter.Output.WriteLine("".PadRight(headerWidth, headerBorder.Value));
            }

            for (int i = 0; i < valueCount; ++i)
            {
                for (int j = 0; j < columns.Length - 1; ++j)
                {
                    Reporter.Output.Write(columns[j][i].PadRight(columnWidths[j]));
                    Reporter.Output.Write(columnPad);
                }

                Reporter.Output.WriteLine(columns[headers.Length - 1][i]);
            }

            if (valueCount == 0)
            {
                Reporter.Output.WriteLine(noItemsMessage);
            }

            Reporter.Output.WriteLine(" ");
        }
    }
}
