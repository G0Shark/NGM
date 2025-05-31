using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NGMapi.Application.Interfaces;
using NGMapi.Domain.Entities;
using NGMapi.Domain.Interfaces;
using NGMapi.Infrastructure.Repositories;

namespace NGMapi.API.API_v1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IPasswordHasher _hasher;
    private readonly IJwtGenerator _jwtGenerator;
    private readonly IAuthRepository _authRepository;

    public AuthController(IPasswordHasher HASHER, IJwtGenerator JWT_GENERATOR,  IAuthRepository AUTH_REPOSITORY)
    {
        _hasher = HASHER;
        _jwtGenerator = JWT_GENERATOR;
        _authRepository = AUTH_REPOSITORY;
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetJwtToken(string USERNAME, string PASSWORD, bool FOR_LONG)
    {
        User u = _authRepository.GetUserWithPassword(USERNAME);
        if (!_hasher.VerifyPassword(PASSWORD, u.PASSWORD))
            return Unauthorized();

        DateTime time = DateTime.Now;
        if (FOR_LONG)
            return Ok(_jwtGenerator.GenerateJwt(_jwtGenerator.GenerateIdentify(u), time.AddDays(29)));
        
        return Ok(_jwtGenerator.GenerateJwt(_jwtGenerator.GenerateIdentify(u), time.AddDays(1)));
    }
    
    [HttpPost]
    public async Task<ActionResult> Register(User user)
    {
        user.PASSWORD = _hasher.HashPassword(user.PASSWORD);
        _authRepository.Register(user);
        return Ok("Пользователь создан");
    }
}