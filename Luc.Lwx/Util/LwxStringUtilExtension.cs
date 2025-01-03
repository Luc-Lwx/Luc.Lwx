using System.Text;

namespace Luc.Lwx.Util;

public static partial class LwxStringUtilExtension
{
    public static string LwxToHex(this string input)
    {
        char[] chars = input.ToCharArray();
        StringBuilder hex = new();
        foreach (char c in chars)
        {
            hex.Append(((int)c).ToString("x2"));
        }
        return hex.ToString();
    }
}
