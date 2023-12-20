namespace NugetCliSearch.Models;

public class NugetIndex
{
    public string Version { get; set; }
    public List<NugetIndexResource> Resources { get; set; } 
}