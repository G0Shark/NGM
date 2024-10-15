using Microsoft.AspNetCore.Mvc;

namespace NGM_Server.Controllers;

//todo: скоро удалить, ибо тест
[ApiController]
[Route("api/test")]
public class TestController : Controller
{
    [HttpPost("post")]
    public IActionResult HeaderTest([FromBody] Test t)
    {
        if (t.Name == "Oleg")
        {
            return BadRequest("simga");
        }
        else
        {
            return BadRequest("No olegs");
        }
    }
    
    [HttpPut("put")]
    public IActionResult HeaderTest()
    {
        if (Request.Headers.TryGetValue("TestName", out var customHeaderValue))
        {
            return BadRequest("wow your head is " + customHeaderValue);
        }
        else
        {
            return BadRequest("no heads");
        }
    }
}

public class Test
{
    public string Name { get; set; }
    public int Value { get; set; }
}