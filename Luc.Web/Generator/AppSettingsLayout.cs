using System.Text.Json.Serialization;

namespace Luc.Web.Generator;


internal class AppSettingsLayout
{
  [JsonPropertyName("LucWeb")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
  public AppSettingsSectionLayout? LucWeb { get; set; }
}

internal class AppSettingsSectionLayout
{
  [JsonIgnore]
  public bool FromFile { get; set; }

  [JsonPropertyName("ApiManagerPath")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
  public string? ApiManagerPath { get; set; }

  [JsonPropertyName("SwaggerDescription")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
  public string? SwaggerDescription { get; set; }

  [JsonPropertyName("SwaggerContactEmail")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
  public string? SwaggerContactEmail { get; set; }

  [JsonPropertyName("SwaggerContactPhone")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
  public string? SwaggerContactPhone { get; set; }

  [JsonPropertyName("SwaggerAuthor")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
  public string? SwaggerAuthor { get; set; }
}
  