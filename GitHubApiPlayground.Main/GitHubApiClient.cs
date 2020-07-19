using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
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
        }

        public async Task<List<Repo>> GetRepos(string uri)
        {
            var response = _httpClient.GetStreamAsync(uri);
			return await JsonSerializer.DeserializeAsync<List<Repo>>(await response);
        }

        public void PrintSecret()
        {
            Console.WriteLine($"Token is {_settings.Token}");
        }
    }
}