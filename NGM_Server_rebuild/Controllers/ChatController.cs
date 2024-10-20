using Microsoft.AspNetCore.Mvc;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatController : Controller
{
    [HttpPost]
    public ActionResult<string> CreateChat(string receiver)
    {
        return Ok("Hello World!");
    }
}