using System.Security.Cryptography;
using System.Text;

namespace TalyerApp.Domain.Shared.Membership;

public static class EmailVerificationToken
{
    public static string GeneratePlaintext() => Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

    public static string Hash(string plaintext)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(plaintext));
        return Convert.ToHexString(bytes);
    }
}
