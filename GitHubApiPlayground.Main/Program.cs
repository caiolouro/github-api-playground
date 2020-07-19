using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GitHubApiPlayground
{
    public class Program
    {
		public static IConfigurationRoot Configuration { get; set; }

		static async Task Main(string[] args)
		{
			var builder = new ConfigurationBuilder();
			builder.AddUserSecrets<Program>();
			Configuration = builder.Build();

			var services = new ServiceCollection()
				.Configure<GitHubSettings>(Configuration.GetSection(nameof(GitHubSettings)))
				.AddSingleton<GitHubApiClient>()
				.BuildServiceProvider();

			var githubApiClient = services.GetService<GitHubApiClient>();

			var repos = await githubApiClient.GetRepos("https://api.github.com/users/caiolouro/repos");
			foreach (var repo in repos)
			{
				Console.WriteLine(repo.Name);
				Console.WriteLine(repo.URL);
				Console.WriteLine(repo.LastPushedAt);
				Console.WriteLine();
			}

			Console.WriteLine("Hello World!");
		}
    }
}