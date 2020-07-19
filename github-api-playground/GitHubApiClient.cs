using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GitHubAPIPlayground
{
    public static class GitHubApiClient
    {
        private static readonly HttpClient _httpClient;

        static GitHubApiClient()
        {
            _httpClient = new HttpClient();
			_httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "caiolouro");
        }

		public static Task<Stream> GetStream(string uri) {
			return _httpClient.GetStreamAsync(uri);
		}
    }
}