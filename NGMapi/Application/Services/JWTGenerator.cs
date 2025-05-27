using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NGM_Server_rebuild.Domain.Entities;
using NGMapi.Application.Interfaces;
using NGMapi.Domain.Entities;

namespace NGMapi.Application.Services;

public class JwtGenerator : IJwtGenerator
{
    public ClaimsIdentity GenerateIdentify(User u)
    {
        var claims = new List<Claim>
        {
            new(ClaimsIdentity.DefaultNameClaimType, u.USERNAME),
            new(ClaimsIdentity.DefaultRoleClaimType, u.ID.ToString())
        };
        ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
        return claimsIdentity;
    }

    public string GenerateJwt(ClaimsIdentity identity, DateTime expires)
    {
        var now = DateTime.UtcNow;
        var jwt = new JwtSecurityToken(
            issuer: "NGMApiV1",
            audience: "NGMClient",
            notBefore: now,
            claims: identity.Claims,
            expires: expires, 
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.ASCII
                        .GetBytes("secret_ultra_key_mega3957239587weasjrgfhnolrv3uh")),
                    SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}