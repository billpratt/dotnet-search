namespace NugetCliSearch.Models;

public class NugetIndexResource
{
    [JsonProperty("@id")] public string Id { get; set; }
    [JsonProperty("@type")] public string Type { get; set; }
    public string Comment { get; set; }
}