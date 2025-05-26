using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using NGMapi.Application.Interfaces;

namespace NGMapi.Application.Services;

public class Argon2PasswordHasher : IPasswordHasher
{
    private readonly IConfiguration _config;

    public Argon2PasswordHasher(IConfiguration config)
    {
        _config = config;
    }

    public async Task<string> HashPasswordAsync(string password)
    {
        var pepper = "super_secrets";
        var salt = GenerateSalt();
        
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password + pepper))
        {
            Salt = salt,
            Iterations = 1000,
            MemorySize = 65536,
            DegreeOfParallelism = 4
        };

        var hash = await argon2.GetBytesAsync(128);
        return FormatHash(hash, salt);
    }

    public async Task<bool> VerifyPasswordAsync(string password, string hash)
    {
        var (storedHash, salt) = ParseHash(hash);
        var pepper = "super_secrets";
        
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password + pepper))
        {
            Salt = salt,
            Iterations = 1000,
            MemorySize = 65536,
            DegreeOfParallelism = 4
        };

        var newHash = await argon2.GetBytesAsync(128);
        return CryptographicOperations.FixedTimeEquals(storedHash, newHash);
    }

    private byte[] GenerateSalt()
    {
        var salt = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return salt;
    }

    private string FormatHash(byte[] hash, byte[] salt)
    {
        return $"{Convert.ToBase64String(hash)}:{Convert.ToBase64String(salt)}";
    }

    private (byte[] hash, byte[] salt) ParseHash(string input)
    {
        var parts = input.Split(':', 2);
        return (
            Convert.FromBase64String(parts[0]),
            Convert.FromBase64String(parts[1])
        );
    }
}