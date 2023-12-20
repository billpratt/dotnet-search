using NugetCliSearch.Services;

namespace NugetCliSearch;

public class Program
{
    [Conditional("DEBUG")]
    private static void ReadArgs(ref string[] args)
    {
        var input = Prompt.GetString("> ");
        var inputSplit = input?.Split(' ') ?? new string[1];

        switch (inputSplit[0])
        {
            case "clear":
                Console.Clear();
                break;
            case "exit":
                // Exit out
                Environment.Exit(0);
                break;
            default:
                args = inputSplit;
                break;
        }
    }

    public static async Task<int> Main(string[] args)
    {
        // for debugging purposes
        ReadArgs(ref args);

        try
        {
            return await Host.CreateDefaultBuilder(args)
                .UseConsoleLifetime()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    if (!args.Contains("--verbose") && !args.Contains("-v")) return;
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Debug);
                })
                .ConfigureServices((_, services) =>
                {
                    services.AddHttpClient<SearchCommand>();
                    services.AddSingleton<NugetIndexReader>();
                })
                .RunCommandLineApplicationAsync<SearchCommand>(args)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error {e.Message}");
            return 1;
        }
    }
}