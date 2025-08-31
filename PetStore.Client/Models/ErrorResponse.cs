using System.Text.Json.Serialization;

namespace PetStore.Client.Models;

public class ErrorResponse
{
    [JsonPropertyName("code")] public int? Code { get; set; }

    [JsonPropertyName("type")] public string? Type { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }
}