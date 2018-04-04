using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetSearch
{
    class Program
    {
        static int Main(string[] args)
        {
#if DEBUG
            while (true)
            {
                var input = Prompt.GetString("> ");
                var inputSplit = input?.Split(' ') ?? new string[1];

                if (inputSplit[0] == "clear")
                {
                    Console.Clear();
                    continue;
                }
                else if (inputSplit[0] == "exit")
                {
                    // Exit out
                    return 0;
                }

                var result = RunApp(inputSplit).GetAwaiter().GetResult();
            }
#else
            return RunApp(args).GetAwaiter().GetResult();
#endif
        }

        private static async Task<int> RunApp(string[] args)
        {
            return await CommandLineApplication.ExecuteAsync<SearchCommand>(args);
        }
    }
}
