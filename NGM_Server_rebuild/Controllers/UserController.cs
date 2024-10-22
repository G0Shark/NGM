using Microsoft.AspNetCore.Mvc;

namespace NGM_Server_rebuild.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{
    
}

public static class UserControllerExtensions
{
    public static bool IsUserExist(string Username)
    {
        return false;
    }
    
}