using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NGM_Server.Models;

namespace NGM_Server.Controllers;

public class DBController : Controller
{
    private readonly MyDBContext DBContext;

    public DBController(MyDBContext DBContext)
    {
        this.DBContext = DBContext;
    }
    
    [HttpGet("")]
    public async Task<ActionResult<List<User>>> Get()
    {
        var List = await DBContext.Users.Select(
            s => new User
            {
                Name = s.Name,
                password = s.password,
                description = s.description,
                uniqueid = s.uniqueid
            }
        ).ToListAsync();

        if (List.Count < 0)
        {
            return NotFound();
        }
        else
        {
            return List;
        }
    }
}