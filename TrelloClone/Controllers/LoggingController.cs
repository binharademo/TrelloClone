using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TrelloClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoggingController : ControllerBase
    {
        private readonly ILogger<LoggingController> _logger;

        public LoggingController(ILogger<LoggingController> logger)
        {
            _logger = logger;
        }

        [HttpPost("clientlog")]
        public async Task<IActionResult> LogClientMessage([FromBody] ClientLogModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Content))
            {
                return BadRequest("Log content is required");
            }

            try
            {
                // Sanitize the file path to prevent directory traversal
                string fileName = Path.GetFileName(model.FilePath);
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = "client_logs.txt";
                }

                // Ensure the log directory exists
                string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "Logs");
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                // Write to the log file
                string logFilePath = Path.Combine(logDirectory, fileName);
                await System.IO.File.AppendAllTextAsync(logFilePath, model.Content);

                _logger.LogInformation($"Client log written to {logFilePath}");
                return Ok(new { success = true, filePath = logFilePath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing client log");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class ClientLogModel
    {
        public string FilePath { get; set; }
        public string Content { get; set; }
    }
}
