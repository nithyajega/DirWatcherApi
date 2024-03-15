using System;
using System.IO;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace DirWatcherApi.Services
{
    public class FileWatcherService
    {
        private readonly string _directoryPath;
        private readonly string _connectionString;
        private readonly string _your_magic_string;
        private Timer _timer;
        private CancellationTokenSource _cancellationTokenSource;
        FileSystemWatcher watcher = new FileSystemWatcher();

        public FileWatcherService(IConfiguration configuration)
        {
            _directoryPath = configuration["DirectoryPath"];
            _connectionString = configuration.GetConnectionString("DirWatcherConnectionString");
            _your_magic_string = configuration["your_magic_string"];
        }

        public void StartWatching()
        {

            StartManualMonitoring();

            StartScheduledMonitoring();            
        }

        public void StopWatching()
        {
            // Stop manual monitoring
            StopManualMonitoring();

            // Stop scheduled monitoring
            StopScheduledMonitoring();
        }

        private void StartManualMonitoring()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = _directoryPath;
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.*";
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Changed += OnChanged;
            watcher.EnableRaisingEvents = true;
        }

        private void StopManualMonitoring()
        {
            try
            {
                if (watcher != null)
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                    watcher = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping manual monitoring: {ex.Message}");
            }
        }

        public void StartScheduledMonitoring()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _timer = new Timer(PerformScheduledMonitoring, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        }

        private void PerformScheduledMonitoring(object state)
        {
            try
            {
                StartManualMonitoring();
                int occurrences = CountMagicStringOccurrences();
                LogOccurrenceCount(_directoryPath, _your_magic_string, occurrences); // Assuming magic string is constant for scheduled monitoring
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error performing scheduled monitoring: {ex.Message}");
            }
        }

        private void StopScheduledMonitoring()
        {
            _cancellationTokenSource.Cancel();
            _timer.Dispose();
        }

        

        private int CountMagicStringOccurrences()
        {
            int totalCount = 0;
            foreach (string filePath in Directory.GetFiles(_directoryPath))
            {
                string content = File.ReadAllText(filePath);
                totalCount += CountOccurrences(content, _your_magic_string);
            }
            return totalCount;
        }

        private int CountOccurrences(string content, string magicString)
        {
            // Implementation of counting occurrences of magic string in content
            // Replace this with your own implementation
            int count = 0;
            int index = content.IndexOf(magicString);
            while (index != -1)
            {
                count++;
                index = content.IndexOf(magicString, index + 1);
            }
            return count;
        }

        private void LogOccurrenceCount(string filePath, string magicString, int count)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO OccurrenceLogs (FilePath, MagicString, OccurrenceCount, LogTime) VALUES (@FilePath, @MagicString, @OccurrenceCount, @LogTime)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@FilePath", filePath);
                    command.Parameters.AddWithValue("@MagicString", magicString);
                    command.Parameters.AddWithValue("@OccurrenceCount", count);
                    command.Parameters.AddWithValue("@LogTime", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging occurrence count: {ex.Message}");
            }
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            LogToFileChange(e.ChangeType, e.FullPath);
        }

        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            LogToFileChange(e.ChangeType, e.FullPath);
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            LogToFileChange(e.ChangeType, e.FullPath);
        }


        public Dictionary<string, int> SearchMagicStringOccurrences(string magicString)
        {
            Dictionary<string, int> occurrences = new Dictionary<string, int>();

            foreach (string filePath in Directory.GetFiles(_directoryPath))
            {
                int count = CountOccurrences(File.ReadAllText(filePath), magicString);
                occurrences.Add(Path.GetFileName(filePath), count);
                if (count > 0)
                {
                    LogOccurrenceCount(filePath, magicString, count);
                }
            }

            return occurrences;
        }

       
        private void LogToFileChange(WatcherChangeTypes changeType, string filePath)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO FileChanges (ChangeType, FilePath, ChangeTime) VALUES (@ChangeType, @FilePath, @ChangeTime)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ChangeType", changeType.ToString());
                    command.Parameters.AddWithValue("@FilePath", filePath);
                    command.Parameters.AddWithValue("@ChangeTime", DateTime.Now);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging file change: {ex.Message}");
            }
        }
    }
}
