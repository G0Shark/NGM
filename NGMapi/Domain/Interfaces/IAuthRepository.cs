using NGMapi.Domain.Entities;

namespace NGMapi.Domain.Interfaces;

public interface IAuthRepository
{
    public void Register(User user);
}