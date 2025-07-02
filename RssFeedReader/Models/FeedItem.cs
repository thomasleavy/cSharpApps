namespace RssFeedReader.Models
{
    public class FeedItem
    {
        public string Source { get; set; }    // e.g. “NYTimes”
        public string Title { get; set; }
        public string Link { get; set; }
        public DateTime PublishDate { get; set; }
    }
}
