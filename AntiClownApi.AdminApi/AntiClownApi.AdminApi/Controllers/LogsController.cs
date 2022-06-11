using AntiClownApiClient.Dto.Log;
using Microsoft.AspNetCore.Mvc;

namespace AntiClownApi.AdminApi.Controllers;

[ApiController]
[Route("/adminApi/logs")]
public class LogsController : Controller
{
    public LogsController()
    {
        
    }
    
    [HttpGet]
    public string[] ReadLogCategories()
    {
        return Array.Empty<string>();
    }

    [HttpPost("filter")]
    public string[] FindLogs([FromBody] LogFilter filter)
    {
        return Array.Empty<string>();
    }

    [HttpGet("{category}/files/{fileName}")]
    public string ReadFile(string category, string fileName)
    {
        return string.Empty;
    }
}