using System.Text.Json.Serialization;

namespace PetStore.Client.Models;

// Model odpowiedzi błędu (np. 400, 404) z Petstore
public class ErrorResponse
{
    [JsonPropertyName("code")] public int? Code { get; set; }

    [JsonPropertyName("type")] public string? Type { get; set; }

    [JsonPropertyName("message")] public string? Message { get; set; }
}