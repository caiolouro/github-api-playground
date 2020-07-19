using System;
using System.Collections.Generic;
using System.IO;
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
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "caiolouro");

            var authToken = Encoding.ASCII.GetBytes($"caiolouro:{_settings.Token}");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
        }

        public async Task<List<Repo>> GetRepos(string username)
        {
            var response = _httpClient.GetStreamAsync($"https://api.github.com/users/{username}/repos");
            return await JsonSerializer.DeserializeAsync<List<Repo>>(await response);
        }

        public async Task<List<Repo>> GetWatchedRepos(string username)
        {
            // var response = await _httpClient.GetAsync($"https://api.github.com/users/{username}/subscriptions?page=4");

			// var nextUri = GetNextUriFromLinkResponseHeader(response.Headers.GetValues("Link").FirstOrDefault());
			// Console.WriteLine($"nextUri {nextUri}");

            // return await JsonSerializer.DeserializeAsync<List<Repo>>(await response.Content.ReadAsStreamAsync());

            // var response = _httpClient.GetStreamAsync($"https://api.github.com/users/{username}/subscriptions");
            // response.Headers
            // return await JsonSerializer.DeserializeAsync<List<Repo>>(await response);

			var repos = new List<Repo>();

			var uri = $"https://api.github.com/users/{username}/subscriptions?per_page=100";
			Console.WriteLine($"firstUri {uri}");

			while (uri != null)
			{
				var response = await _httpClient.GetAsync(uri);
				var reposChunk = await JsonSerializer.DeserializeAsync<List<Repo>>(await response.Content.ReadAsStreamAsync());
				repos.AddRange(reposChunk.ToList());

				uri = GetNextUriFromLinkResponseHeader(response.Headers.GetValues("Link").FirstOrDefault());
				Console.WriteLine($"nextUri {uri}");
			}

			Console.WriteLine($"repos {repos.Count}");
			return repos;
        }

        public async Task<List<Repo>> GetWatchedReposFromAuthUser()
        {
            var response = _httpClient.GetStreamAsync($"https://api.github.com/user/subscriptions");
            return await JsonSerializer.DeserializeAsync<List<Repo>>(await response);
        }

        public void PrintSecret()
        {
            Console.WriteLine($"Token is {_settings.Token}");
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