using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using RssFeedReader.Models;

namespace RssFeedReader.Services
{
    public class FeedService
    {
        private readonly HttpClient _http = new();
        private readonly string _configPath;

        public FeedService(string configPath) => _configPath = configPath;

        public async Task<IEnumerable<string>> LoadFeedUrlsAsync()
        {
            var json = await File.ReadAllTextAsync(_configPath);
            return JsonSerializer.Deserialize<List<string>>(json)
                   ?? Enumerable.Empty<string>();
        }

        public async Task<List<FeedItem>> FetchFeedAsync(string url)
        {
            try
            {
                var xml = await _http.GetStringAsync(url);
                var doc = XDocument.Parse(xml);
                var title = doc.Root?
                    .Element("channel")?
                    .Element("title")?
                    .Value ?? url;

                return doc
                    .Descendants("item")
                    .Select(item => new FeedItem
                    {
                        Source = title,
                        Title = item.Element("title")?.Value ?? "(no title)",
                        Link  = item.Element("link")?.Value  ?? "",
                        PublishDate = DateTime.TryParse(
                            item.Element("pubDate")?.Value, 
                            out var dt) ? dt : DateTime.MinValue
                    })
                    .ToList();
            }
            catch
            {
                return new List<FeedItem>(); // on error, skip
            }
        }
    }
}
