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

			var repos = await githubApiClient.GetWatchedByAuthUserRepos();

			// Unwatch repos that I'm not working anymore
			foreach (var repo in repos)
			{
				Console.WriteLine(repo.FullName);

				if (repo.Owner.Login.Equals("vtex") || repo.Owner.Login.Equals("vtex-gocommerce") || repo.Owner.Login.Equals("mlcunha"))
				{
					if (!repo.FullName.Equals("vtex/catalog")) await githubApiClient.DeleteRepoSubscriptionForAuthUser(repo);					
				}
			}
		}
    }
}