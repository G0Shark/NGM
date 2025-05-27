using System.Security.Claims;
using NGM_Server_rebuild.Domain.Entities;
using NGMapi.Domain.Entities;

namespace NGMapi.Application.Interfaces;

public interface IJwtGenerator
{
    public ClaimsIdentity GenerateIdentify(User u);
    public string GenerateJwt(ClaimsIdentity identity, DateTime expires);
}