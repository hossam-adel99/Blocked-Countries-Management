using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/logs")]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _log;
        public LogsController(ILogService log) => _log = log;

        [HttpGet("blocked-attempts")]
        public IActionResult GetBlockedAttempts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var all = _log.GetLogs();
            var total = all.Count();
            var items = all.Skip((page - 1) * pageSize).Take(pageSize);
            return Ok(new { Total = total, Page = page, PageSize = pageSize, Items = items });
        }
    }

}
