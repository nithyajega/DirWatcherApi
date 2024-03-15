using DirWatcherApi.Models;

namespace DirWatcherApi.Services
{
    public interface IDbService
    {
        void SaveTaskRun(TaskRun taskRun);
        List<TaskRun> GetTaskRuns();
    }
}
