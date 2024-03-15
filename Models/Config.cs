namespace DirWatcherApi.Models
{
    public class Config
    {
        public string Directory { get; set; }
        public TimeSpan Interval { get; set; }
        public string MagicString { get; set; }
        public string Database { get; set; }
    }
}
