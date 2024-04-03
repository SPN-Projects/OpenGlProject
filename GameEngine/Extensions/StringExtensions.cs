using System.Security.Cryptography;
using System.Text;

namespace GameEngine.Extensions;
public static class StringExtensions
{
    public static string CreateHash(this string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);

        return Convert.ToBase64String(hash);
    }
}
