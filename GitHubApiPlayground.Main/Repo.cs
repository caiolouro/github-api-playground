using System;
using System.Text.Json.Serialization;

namespace GitHubApiPlayground
{
    public class Repo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("full_name")]
        public string FullName { get; set; }

        [JsonPropertyName("html_url")]
        public Uri URL { get; set; }

        [JsonPropertyName("pushed_at")]
        public DateTime LastPushedAtUtc { get; set; }

        public DateTime LastPushedAt => LastPushedAtUtc.ToLocalTime();

        [JsonPropertyName("owner")]
        public RepoOwner Owner { get; set; }
    }
}