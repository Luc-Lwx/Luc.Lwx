using System.Text.Json.Serialization;

namespace Luc.Lwx.Generator;


internal class AppSettingsDto
{
  [JsonPropertyName("Lwx")] [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
  public AppSettingsSectionDto? Lwx { get; set; }
}

internal class AppSettingsSectionDto
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
  