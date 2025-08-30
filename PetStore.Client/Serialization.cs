using System.Text.Json;

namespace PetStore.Client;

public static class Serialization
{
    // Shared JSON options for all API clients
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true
    };
}