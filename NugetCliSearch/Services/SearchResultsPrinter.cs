namespace NugetCliSearch.Services;

public class SearchResultsPrinter
{
    
    public static void PrintAsList(SearchResult searchResult)
    {
        foreach (var data in searchResult.Data)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(data.Id);
            Console.ResetColor();
            Console.WriteLine($" by {string.Join(",", data.Authors)}\tv{data.Version}");
            Console.WriteLine($"{data.Description}{Environment.NewLine}");
            Console.WriteLine($"Tags: {string.Join(',', data.Tags)}");
            Console.WriteLine($"Downloads: {data.TotalDownloads.Abbreviate()}");
            Console.WriteLine($"{Environment.NewLine}----------------------{Environment.NewLine}");
        }
    }

    public static void PrintAsTable(SearchResult searchResponse)
    {
        var doc = new Document();
        var tab = new Grid
        {
            Stroke = LineThickness.Single,
            Columns =
            {
                new Column { Width = GridLength.Auto },
                new Column { Width = GridLength.Auto },
                new Column { Width = GridLength.Auto },
                new Column { Width = GridLength.Auto },
                new Column { Width = GridLength.Auto },
                new Column { Width = GridLength.Auto },
                new Column { Width = GridLength.Auto },
                new Column { Width = GridLength.Auto }
            },
            AutoPosition = true,
            Name = "Nuget Results"
        };

        tab.Children.Add(new Cell("Name") { Stroke = LineThickness.Single, Color = ConsoleColor.Yellow });
        tab.Children.Add(new Cell("Description") { Stroke = LineThickness.Single, Color = ConsoleColor.Yellow });
        tab.Children.Add(new Cell("Authors") { Stroke = LineThickness.Single, Color = ConsoleColor.Yellow });
        tab.Children.Add(new Cell("Package Type") { Stroke = LineThickness.Single, Color = ConsoleColor.Yellow });
        tab.Children.Add(new Cell("Tags") { Stroke = LineThickness.Single, Color = ConsoleColor.Yellow });
        tab.Children.Add(new Cell("Version")
        {
            Stroke = LineThickness.Single,
            Color = ConsoleColor.Yellow,
            Padding = new Thickness(2, 0)
        });
        tab.Children.Add(new Cell("Downloads")
        {
            Stroke = LineThickness.Single,
            Color = ConsoleColor.Yellow,
            Padding = new Thickness(2, 0)
        });
        tab.Children.Add(new Cell("Verified")
        {
            Stroke = LineThickness.Single,
            Color = ConsoleColor.Yellow,
            Padding = new Thickness(2, 0)
        });

        searchResponse.Data.ToList().ForEach(x =>
        {
            tab.Children.Add(new Cell(x.Id) { Stroke = LineThickness.Single });
            tab.Children.Add(new Cell(x.Description)
            {
                Stroke = LineThickness.Single,
                TextWrap = TextWrap.WordWrap,
                Width = 50
            });
            tab.Children.Add(new Cell(string.Join(", ", x.Authors)) { Stroke = LineThickness.Single,
            TextWrap = TextWrap.WordWrap,
            Width = 25 });
            tab.Children.Add(new Cell(string.Join(", ", x.PackageTypes.Select(y => y.Name)))
            {
                Stroke = LineThickness.Single,
                TextWrap = TextWrap.WordWrap,
                Padding = new Thickness(5, 0)
            });
            tab.Children.Add(new Cell(string.Join(", ", x.Tags))
            {
                Stroke = LineThickness.Single,
                TextWrap = TextWrap.WordWrap,
                Width = 25
            });
            tab.Children.Add(new Cell(x.Version) { Stroke = LineThickness.Single, TextAlign = TextAlign.Center });
            tab.Children.Add(new Cell(x.TotalDownloads.Abbreviate()) { Stroke = LineThickness.Single, TextAlign = TextAlign.Center });
            tab.Children.Add(new Cell(x.Verified ? "yes" : "no")
            {
                Stroke = LineThickness.Single,
                TextAlign = TextAlign.Center,
                Color = x.Verified ? ConsoleColor.Green : ConsoleColor.Red
            });
        });

        doc.Children.Add(tab);

        ConsoleRenderer.RenderDocument(doc);
    }
}