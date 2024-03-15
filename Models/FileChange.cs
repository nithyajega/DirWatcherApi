using System;

namespace DirWatcherApi.Models
{

    public class FileChange
    {
        public int Id { get; set; }
        public string ChangeType { get; set; }
        public string FilePath { get; set; }
        public DateTime ChangeTime { get; set; }
    }

}
