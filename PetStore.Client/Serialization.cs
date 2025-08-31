using System.Text.Json;

namespace PetStore.Client;

public static class Serialization
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true
    };
}