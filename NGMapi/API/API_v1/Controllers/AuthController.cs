using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NGMapi.Application.Interfaces;
using NGMapi.Domain.Entities;
using NGMapi.Infrastructure.Repositories;

namespace NGMapi.API.API_v1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IPasswordHasher hasher;
    private readonly IJwtGenerator jwtGenerator;

    public AuthController(IPasswordHasher hasher, IJwtGenerator jwtGenerator)
    {
        this.hasher = hasher;
        this.jwtGenerator = jwtGenerator;
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetJwtToken(string USERNAME, string PASSWORD, bool FOR_LONG)
    {
        User u = AuthRepository.GetUserWithPassword(USERNAME);
        if (!await hasher.VerifyPasswordAsync(PASSWORD, u.PASSWORD))
            return Unauthorized();

        DateTime time = DateTime.Now;
        time.AddHours(8);
        if (FOR_LONG)
            time.AddDays(29);

        u.PASSWORD = "";
        
        var res = new
        {
            token = jwtGenerator.GenerateJwt(jwtGenerator.GenerateIdentify(u), time),
            user = u
        };
        
        return Ok(res);
    }
    
    [HttpPost]
    public async Task<ActionResult> Register(string USERNAME, string PASSWORD, string EMAIL, string? DISPLAYNAME, string? DESCRIPTION)
    {
        AuthRepository.Register(USERNAME, await hasher.HashPasswordAsync(PASSWORD), EMAIL, DISPLAYNAME, DESCRIPTION);
        return Ok("Пользователь создан");
    }
}