using System.Text.Json;

namespace Luc.Web.Util;


public class LucUtilLowercaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return name.ToLower();
    }
}