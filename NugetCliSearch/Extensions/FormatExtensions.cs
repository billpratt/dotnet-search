namespace NugetCliSearch.Extensions;

public static class FormatExtensions
{
    public static string Abbreviate(this TotalDownloads value)
    {
        return value switch
        {
            > 999999999 => value.ToString("0,,,.##B", CultureInfo.InvariantCulture),
            > 999999 => value.ToString("0,,.#0M", CultureInfo.InvariantCulture),
            > 999 => value.ToString("0,.#0K", CultureInfo.InvariantCulture),
            _ => value.ToString()
        };
    }
}