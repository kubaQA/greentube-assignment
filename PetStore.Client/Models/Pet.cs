using System.Text.Json.Serialization;

namespace PetStore.Client.Models;

public class Category
{
    [JsonPropertyName("id")]
    public long? Id { get; set; } // id shouldn't be modified on client site, it should be created on backend, bug

    [JsonPropertyName("name")]
    public string? Name { get; set; } // there should be a determined categories list we could provide, bug
}

public class Tag
{
    [JsonPropertyName("id")] // same story with id
    public long? Id { get; set; }

    [JsonPropertyName("name")] // there should be a list of tags, bug
    public string? Name { get; set; }
}

public class Pet
{
    [JsonPropertyName("id")] // id shouldn't be modified on client site, it should be created on backend, bug
    public long? Id { get; set; } // 

    [JsonPropertyName("category")] public Category? Category { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; } = "";

    [JsonPropertyName("photoUrls")] // maybe some regex to avoid adding urls which are not images, bug
    public List<string> PhotoUrls { get; set; } = new();

    [JsonPropertyName("tags")] public List<Tag>? Tags { get; set; }

    [JsonPropertyName(
        "status")] // status is determined in get pet/findByStatus, but here i can provide whatever i want, bug
    public string? Status { get; set; }
}