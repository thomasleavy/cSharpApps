/*To compile: dotnet run*/
using System;
using System.Linq;
using System.Threading.Tasks;
using RssFeedReader.Services;

namespace RssFeedReader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var service = new FeedService("feeds.json");
            var urls = await service.LoadFeedUrlsAsync();

            Console.WriteLine("Fetching feeds...\n");

            // Fetch all feeds in parallel
            var allItems = await Task.WhenAll(
                urls.Select(u => service.FetchFeedAsync(u))
            );

            // Flatten & sort by most recent
            var latest = allItems
                .SelectMany(list => list)
                .OrderByDescending(item => item.PublishDate)
                .Take(20);

            Console.WriteLine("=== Latest 20 Items ===\n");
            foreach (var item in latest)
            {
                Console.WriteLine($"{item.PublishDate:yyyy-MM-dd} [{item.Source}]");
                Console.WriteLine($"  {item.Title}");
                Console.WriteLine($"  {item.Link}\n");
            }

            Console.WriteLine("Done. Press any key to exit.");
            Console.ReadKey();
        }
    }
}
