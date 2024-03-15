using Microsoft.AspNetCore.Mvc;
using DirWatcherApi.Services;

namespace DirWatcherApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileWatcherController : ControllerBase
    {
        private readonly FileWatcherService _fileWatcherService;

        public FileWatcherController(FileWatcherService fileWatcherService)
        {
            _fileWatcherService = fileWatcherService;
        }

        [HttpPost("start")]
        public IActionResult StartWatching()
        {
            _fileWatcherService.StartWatching();
            return Ok("File watching started.");
        }

        [HttpPost("stop")]
        public IActionResult StopWatching()
        {
            _fileWatcherService.StopWatching();
            return Ok("File watching stopped.");
        }

        [HttpPost("search")]
        public IActionResult SearchMagicString([FromBody] string magicString)
        {
            if (string.IsNullOrEmpty(magicString))
            {
                return BadRequest("Magic string cannot be empty.");
            }

            var result = _fileWatcherService.SearchMagicStringOccurrences(magicString);
            return Ok(result);
        }
    }
}
