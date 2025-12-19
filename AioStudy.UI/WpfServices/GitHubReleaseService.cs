using AioStudy.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AioStudy.UI.WpfServices
{
    public class GitHubReleaseService : IGitHubReleaseService
    {
        private const string Url ="https://api.github.com/repos/MilanMINT/AioStudyClientWin/releases/latest";

        public async Task<(string Tag, string Url)?> GetLatestReleaseTagAsync()
        {
            try
            {
                using var client = new HttpClient();

                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("AioStudyClientWin", "1.0"));

                var json = await client.GetStringAsync(Url);

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                string? tag = root.GetProperty("tag_name").GetString();
                string? htmlUrl = root.GetProperty("html_url").GetString();

                if (tag == null || htmlUrl == null)
                    return null;

                return (tag, htmlUrl);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
