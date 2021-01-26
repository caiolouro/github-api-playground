using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace GitHubApiPlayground
{
    public class GitHubApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly GitHubSettings _settings;

        public GitHubApiClient(IOptions<GitHubSettings> settings)
        {
            _settings = settings.Value;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", _settings.Username);

            var authToken = Encoding.ASCII.GetBytes($"{_settings.Username}:{_settings.Token}");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
        }

        public async Task<List<Repo>> GetUserRepos(string username)
        {
            var response = _httpClient.GetStreamAsync($"https://api.github.com/users/{username}/repos");
            return await JsonSerializer.DeserializeAsync<List<Repo>>(await response);
        }

        public async Task<List<Repo>> GetWatchedByAuthUserRepos()
        {
            Console.WriteLine("Fetching watched repos...");
            var repos = new List<Repo>();

            var uri = $"https://api.github.com/user/subscriptions?per_page=100";

            while (uri != null)
            {
                var response = await _httpClient.GetAsync(uri);
                var reposChunk = await JsonSerializer.DeserializeAsync<List<Repo>>(await response.Content.ReadAsStreamAsync());
                repos.AddRange(reposChunk.ToList());

                try
                {
                    uri = GetNextUriFromLinkResponseHeader(response.Headers.GetValues("Link").FirstOrDefault());
                }
                catch (Exception)
                {
                    uri = null;
                }
            }

            Console.WriteLine($"{repos.Count} watched repos found.");
            return repos;
        }

        public async Task<string> DeleteRepoSubscriptionForAuthUser(Repo repo)
        {
            var response = await _httpClient.DeleteAsync($"https://api.github.com/repos/{repo.FullName}/subscription");

            if (response.IsSuccessStatusCode) Console.WriteLine($"Subscription deleted for {repo.FullName}.");
            else Console.WriteLine("Error while trying to delete subscription for {repo.FullName}: {response.ReasonPhrase}");

            return response.ReasonPhrase;
        }

        private static string GetNextUriFromLinkResponseHeader(string rawValue)
        {
            string[] linkParts = rawValue.Split(',');

            foreach (var linkPart in linkParts)
            {
                var relMatch = Regex.Match(linkPart, "(?<=rel=\").+?(?=\")", RegexOptions.IgnoreCase);
                var uriMatch = Regex.Match(linkPart, "(?<=<).+?(?=>)", RegexOptions.IgnoreCase);

                if (relMatch.Success && uriMatch.Success)
                {
                    string rel = relMatch.Value.ToUpper();
                    string uri = uriMatch.Value;

                    switch (rel)
                    {
                        case "NEXT":
                            return uri;
                    }
                }
            }

            return null;
        }
    }
}