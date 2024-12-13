using System.Text.Json.Serialization;

namespace Luc.Util.Generator;


internal class AppSettingsDTO
{
  [JsonPropertyName("LucUtil")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
  public AppSettingsSectionDTO? LucUtil { get; set; }
}

internal class AppSettingsSectionDTO
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
  