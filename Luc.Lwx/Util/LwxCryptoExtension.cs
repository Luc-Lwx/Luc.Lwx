using System.Text;
using System.Security.Cryptography;

namespace Luc.Lwx.Util;

public static class LwxCryptoExtension
{
    public static string LwxHashSha1Hex(this string input)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = SHA1.HashData(inputBytes);
        StringBuilder sb = new();
        foreach (byte b in hashBytes)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }
}