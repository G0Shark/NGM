using Microsoft.AspNetCore.Mvc;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
[Route($"api/[controller]")]
public class Test : Controller
{
    public IActionResult Get()
    {
        return Ok("Test connection successful");
    }

    [HttpPost]
    public IActionResult Post(string w)
    {
        return Ok("You writed: " + w);
    }

    [HttpPut]
    public IActionResult Put()
    {
        if (Request.Headers.TryGetValue("test", out var w))
        {
            return Ok("You writed: "+ w);
        }
        else
        {
            return BadRequest("No header found");
        }
    }
    
    
}