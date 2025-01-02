using System.Text;
using System;

namespace Luc.Lwx.Util;

public static class LwxRandomExtension
{
    public const string AlphanumericMulticaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    public const string AlphanumericUpperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    public const string AlphanumericLowerCaseChars = "abcdefghijklmnopqrstuvwxyz0123456789";
    public const string HexChars = "0123456789abcdef";

    public static string LwxGenerateRandomString(this Random random, int length, string chars)
    {
        char[] result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = chars[random.Next(chars.Length)];
        }
        return new string(result);
    }

    public static string LwxRandomHexString(this Random random, int length)
    {
        return random.LwxGenerateRandomString(length, HexChars);
    }

    public static string LwxRandomAlphaNumString(this Random random, int length)
    {
        return random.LwxGenerateRandomString(length, AlphanumericMulticaseChars);
    }

    public static string LwxRandomUpperAlphaNumString(this Random random, int length)
    {
        return random.LwxGenerateRandomString(length, AlphanumericUpperCaseChars);
    }

    public static string LwxRandomLowerAlphaNumString(this Random random, int length)
    {
        return random.LwxGenerateRandomString(length, AlphanumericLowerCaseChars);
    }
}