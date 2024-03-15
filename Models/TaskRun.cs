namespace DirWatcherApi.Models
{
    public class TaskRun
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan TotalRuntime { get; set; }
        public List<string> FilesAdded { get; set; }
        public List<string> FilesDeleted { get; set; }
        public int MagicStringOccurrences { get; set; }
        public string Status { get; set; }
        public TimeSpan Runtime { get; internal set; }
        public string FilePath { get; internal set; }
        public int Occurrences { get; internal set; }
    }
}
