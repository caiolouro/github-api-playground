using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace GitHubAPIPlayground
{
    class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            // Console.WriteLine("Hello World!");

			var repos = await GetRepos();
			foreach (var repo in repos)
			{
				Console.WriteLine(repo.Name);
				Console.WriteLine(repo.URL);
				Console.WriteLine(repo.LastPushedAt);
				Console.WriteLine();
			}
        }

        private static async Task<List<Repo>> GetRepos()
        {
			var reposResponse = GitHubApiClient.GetStream("https://api.github.com/users/caiolouro/repos");
			return await JsonSerializer.DeserializeAsync<List<Repo>>(await reposResponse);
        }
    }
}