using System.Security.Cryptography;
using System.Text;
using TrackIt.Application.Interfaces;

namespace TrackIt.Infrastructure.Services;

public class PasswordService : IPasswordService
{
    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var salt = RandomNumberGenerator.GetBytes(32);
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var saltedPassword = salt.Concat(passwordBytes).ToArray();
        var hash = sha256.ComputeHash(saltedPassword);
        var combined = salt.Concat(hash).ToArray();
        return Convert.ToBase64String(combined);
    }

    public bool VerifyPassword(string password, string hash)
    {
        var combined = Convert.FromBase64String(hash);
        var salt = combined.Take(32).ToArray();
        var storedHash = combined.Skip(32).ToArray();

        using var sha256 = SHA256.Create();
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var saltedPassword = salt.Concat(passwordBytes).ToArray();
        var computedHash = sha256.ComputeHash(saltedPassword);

        return storedHash.SequenceEqual(computedHash);
    }
}
