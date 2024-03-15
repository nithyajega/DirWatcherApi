using DirWatcherApi.Data;
using DirWatcherApi.Models;

namespace DirWatcherApi.Services
{
    public class DbService : IDbService
    {
        private readonly ApplicationDbContext _dbContext;

        public DbService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void SaveTaskRun(TaskRun taskRun)
        {
            _dbContext.TaskRuns.Add(taskRun);
            _dbContext.SaveChanges();
        }

        public List<TaskRun> GetTaskRuns()
        {
            return _dbContext.TaskRuns.ToList();
        }
    }
}

