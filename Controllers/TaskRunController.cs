using DirWatcherApi.Data;
using DirWatcherApi.Models;
using DirWatcherApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace DirWatcherApi.Controllers
{

    //[ApiController]
    //[Route("api/[controller]")]
    //public class TaskRunController : ControllerBase
    //{
    //    private readonly DirWatcherBackgroundService _backgroundService;

    //    public TaskRunController(DirWatcherBackgroundService backgroundService)
    //    {
    //        _backgroundService = backgroundService;
    //    }

    //    [HttpPost("start")]
    //    public async Task<IActionResult> StartBackgroundTask()
    //    {
    //        await _backgroundService.StartAsync(default);
    //        return Ok("Background task started successfully.");
    //    }

    //    [HttpPost("stop")]
    //    public async Task<IActionResult> StopBackgroundTask()
    //    {
    //        await _backgroundService.StopAsync(default);
    //        return Ok("Background task stopped successfully.");
    //    }
    //}

    [ApiController]
    [Route("[controller]")]
    public class TaskRunController : ControllerBase
    {
        private readonly Config _config;
        private readonly TaskService _taskService;
        
        public TaskRunController(Config config, TaskService taskService)
        {
            _config = config;
            _taskService = taskService;
        }

        // POST /task/update_config
        [HttpPost("update_config")]
        public IActionResult UpdateConfig([FromBody] Config newConfig)
        {

            _config.Directory = newConfig.Directory;
            _config.Interval = newConfig.Interval;
            _config.MagicString = newConfig.MagicString;
            _config.Database = newConfig.Database;
            _taskService.UpdateConfig(newConfig);
            return Ok(new { message = "Configuration updated successfully" });
        }

        // GET /task/task_runs
        [HttpGet("task_runs")]
        public IActionResult GetTaskRuns()
        {
            var taskRuns = _taskService.GetTaskRuns();
            return Ok(new { task_runs = taskRuns });
        }
    }

}
