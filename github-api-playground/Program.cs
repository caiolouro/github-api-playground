using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GitHubAPIPlayground
{
    class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            await PrintRepos();
        }

        private static async Task PrintRepos()
        {
        }
    }
}
