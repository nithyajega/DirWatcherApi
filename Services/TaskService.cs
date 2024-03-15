
using DirWatcherApi.Models;
using DirWatcherApi.Services;

namespace DirWatcherApi.Services
{
    public class TaskService
    {
        private readonly Config _config;
        private readonly IDbService _dbService;
        public TaskService(Config config, IDbService dbService)
        {
            _config = config;
            _dbService = dbService;
            StartBackgroundTask();
        }

        private void StartBackgroundTask()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var start = DateTime.UtcNow;
                        var directory = _config.Directory;
                        var magicString = _config.MagicString;

                        // Read directory contents
                        var filesBefore = Directory.GetFiles(directory);

                        // Count occurrences of magic string
                        var count = 0;
                        foreach (var file in filesBefore)
                        {
                            var content = File.ReadAllText(file);
                            count += content.Split(new[] { magicString }, StringSplitOptions.None).Length - 1;
                        }

                        // Save task run details
                        var end = DateTime.UtcNow;
                        var runtime = end - start;
                        var filesAfter = Directory.GetFiles(directory);
                        var filesAdded = new List<string>();
                        var filesDeleted = new List<string>();
                        foreach (var file in filesBefore)
                        {
                            if (!Array.Exists(filesAfter, x => x == file))
                                filesDeleted.Add(file);
                        }
                        foreach (var file in filesAfter)
                        {
                            if (!Array.Exists(filesBefore, x => x == file))
                                filesAdded.Add(file);
                        }
                        var taskRun = new TaskRun
                        {
                            StartTime = start,
                            EndTime = end,
                            TotalRuntime = runtime,
                            FilesAdded = filesAdded,
                            FilesDeleted = filesDeleted,
                            MagicStringOccurrences = count,
                            Status = "Success"
                        };
                        _dbService.SaveTaskRun(taskRun);

                        await Task.Delay(_config.Interval);
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions
                        var taskRun = new TaskRun
                        {
                            StartTime = DateTime.UtcNow,
                            EndTime = DateTime.UtcNow,
                            TotalRuntime = TimeSpan.Zero,
                            FilesAdded = new List<string>(),
                            FilesDeleted = new List<string>(),
                            MagicStringOccurrences = 0,
                            Status = "Failed"
                        };
                        _dbService.SaveTaskRun(taskRun);

                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
            });
        }

        public void UpdateConfig(Config newConfig)
        {
            _config.Directory = newConfig.Directory;
            _config.Interval = newConfig.Interval;
            _config.MagicString = newConfig.MagicString;
            _config.Database = newConfig.Database;
        }

        public List<TaskRun> GetTaskRuns()
        {
            var taskRuns = new List<TaskRun>
            {
                new TaskRun {  StartTime = System.DateTime.Now, EndTime = System.DateTime.Now, Runtime = System.TimeSpan.FromMinutes(10), FilesAdded = {"5"}, FilesDeleted = {"2" }, Occurrences = 20, Status = "Success" },
                new TaskRun {  StartTime = System.DateTime.Now, EndTime = System.DateTime.Now, Runtime = System.TimeSpan.FromMinutes(5), FilesAdded = {"3" }, FilesDeleted = {"1" }, Occurrences = 15, Status = "Failed" }
            };

            return taskRuns;
            //return _dbService.GetTaskRuns();
        }


    }
}

