namespace NugetCliSearch.Models;

public class PackageVersion
{
    [JsonProperty("@id")]
    public string Id { get; set; }
    public string Version { get; set; }
    public int Downloads { get; set; }
}