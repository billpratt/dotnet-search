using System.Collections.Generic;
using Newtonsoft.Json;

namespace DotNetSearch
{
    public class Package
    {
        [JsonProperty("@id")]
        public string ApiId { get; set; }
        
        [JsonProperty("@package")]
        public string Type { get; set; }

        public string Registration { get; set; }
        public string Id { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public string Title { get; set; }
        public string IconUrl { get; set; }
        public string LicenseUrl { get; set; }
        public string ProjectUrl { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public IEnumerable<string> Authors { get; set; }
        public long TotalDownloads { get; set; }
        public bool Verified { get; set; }
        public IEnumerable<PackageVersion> Versions { get; set; }
    }
}