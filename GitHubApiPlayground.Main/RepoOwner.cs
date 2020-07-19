using System.Text.Json.Serialization;

namespace GitHubApiPlayground
{
    public class RepoOwner
    {
        [JsonPropertyName("login")]
        public string Login { get; set; }
    }
}