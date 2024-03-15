using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DirWatcherApi.Models;
using DirWatcherApi.Data;

namespace DirWatcherApi.Services
{
    public class DirWatcherBackgroundService : BackgroundService
    {
        private readonly ILogger<DirWatcherBackgroundService> _logger;
        private readonly Config _config;
        private readonly ApplicationDbContext _dbContext;

        public DirWatcherBackgroundService(
            ILogger<DirWatcherBackgroundService> logger,
            Config config,
            ApplicationDbContext dbContext)
        {
            _logger = logger;
            _config = config;
            _dbContext = dbContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Background service is running...");

                    // Monitor the configured directory
                    string[] files = Directory.GetFiles(_config.Directory, "*", SearchOption.AllDirectories);

                    // Process each file
                    foreach (string file in files)
                    {
                        try
                        {
                            // Read file contents
                            string content = File.ReadAllText(file);

                            // Count occurrences of magic string
                            int occurrences = CountOccurrences(content, _config.MagicString);

                            // Save task run details to the database
                            await SaveTaskRunDetailsAsync(file, occurrences);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error processing file {file}: {ex.Message}");
                        }
                    }

                    // Wait for the configured interval before running again
                    await Task.Delay(_config.Interval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error in background service: {ex.Message}");
                }
            }
        }

        internal void ConfigureDirectory(string directoryPath)
        {
            throw new NotImplementedException();
        }

        internal void ConfigureMagicString(string magicString)
        {
            throw new NotImplementedException();
        }

        internal void ConfigureTimeInterval(TimeSpan timeInterval)
        {
            throw new NotImplementedException();
        }

        private int CountOccurrences(string content, string magicString)
        {
            // Implement logic to count occurrences of magic string in content
            // This could be a simple string search or a more complex algorithm
            // For simplicity, let's use a basic string search here
            int occurrences = 0;
            int index = content.IndexOf(magicString, StringComparison.OrdinalIgnoreCase);
            while (index != -1)
            {
                occurrences++;
                index = content.IndexOf(magicString, index + 1, StringComparison.OrdinalIgnoreCase);
            }
            return occurrences;
        }

        private async Task SaveTaskRunDetailsAsync(string filePath, int occurrences)
        {
            // Save task run details to the database
            TaskRun taskRun = new TaskRun
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
                Runtime = TimeSpan.Zero, // Calculate runtime if needed
                FilePath = filePath,
                Occurrences = occurrences
            };

            _dbContext.TaskRuns.Add(taskRun);
            await _dbContext.SaveChangesAsync();
        }
    }
}
