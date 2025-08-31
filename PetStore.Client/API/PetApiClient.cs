using System.Net;
using System.Text.Json;
using PetStore.Client.Http;
using PetStore.Client.Models;
using RestSharp;

namespace PetStore.Client.API;

public class PetApiClient
{
    private readonly RestClient _client;
    private static readonly JsonSerializerOptions JsonOptions = Serialization.Default;

    public PetApiClient(string baseUrl, Action<string>? log = null)
    {
        var opts = new RestClientOptions(baseUrl);
        if (log != null)
            opts.ConfigureMessageHandler = inner => new LoggingHandler(inner, log);

        _client = new RestClient(opts);
    }

    public async Task<(RestResponse response, Pet? pet, ErrorResponse? error)>
        UpdatePetAsync(Pet petPayload)
    {
        var request = new RestRequest("/pet", Method.Put)
            .AddHeader("Accept", "application/json")
            .AddJsonBody(petPayload);

        var response = await _client.ExecuteAsync(request);

        Pet? pet = null;
        ErrorResponse? err = null;

        if (IsSuccess(response.StatusCode))
            pet = TryDeserialize<Pet>(response.Content);
        else
            err = TryDeserialize<ErrorResponse>(response.Content);

        return (response, pet, err);
    }


    public async Task<(RestResponse response, ErrorResponse? error)> DeletePetAsync(long petId, string? apiKey = null)
    {
        var request = new RestRequest($"/pet/{petId}", Method.Delete)
            .AddHeader("Accept", "application/json");
        
        if (!string.IsNullOrWhiteSpace(apiKey))
            request.AddHeader("api_key", apiKey);

        var response = await _client.ExecuteAsync(request);

        ErrorResponse? err = null;
        if (!IsSuccess(response.StatusCode))
            err = TryDeserialize<ErrorResponse>(response.Content);

        return (response, err);
    }


    public async Task<(RestResponse response, List<Pet>? pets, ErrorResponse? error)>
        FindPetsByStatusAsync(string status)
    {
        var request = new RestRequest("/pet/findByStatus", Method.Get)
            .AddHeader("Accept", "application/json")
            .AddQueryParameter("status", status);

        var response = await _client.ExecuteAsync(request);

        List<Pet>? pets = null;
        ErrorResponse? err = null;

        if (IsSuccess(response.StatusCode))
            pets = TryDeserialize<List<Pet>>(response.Content);
        else
            err = TryDeserialize<ErrorResponse>(response.Content);

        return (response, pets, err);
    }


    public async Task<(RestResponse response, Pet? pet, ErrorResponse? error)>
        CreatePetAsync(Pet petPayload)
    {
        var request = new RestRequest("/pet", Method.Post)
            .AddHeader("Accept", "application/json")
            .AddJsonBody(petPayload);

        var response = await _client.ExecuteAsync(request);

        Pet? pet = null;
        ErrorResponse? err = null;

        if (IsSuccess(response.StatusCode))
            pet = TryDeserialize<Pet>(response.Content);
        else
            err = TryDeserialize<ErrorResponse>(response.Content);

        return (response, pet, err);
    }

    public async Task<(RestResponse response, Pet? pet, ErrorResponse? error)>
        GetPetByIdAsync(long petId)
    {
        var request = new RestRequest($"/pet/{petId}", Method.Get)
            .AddHeader("Accept", "application/json");

        var response = await _client.ExecuteAsync(request);

        Pet? pet = null;
        ErrorResponse? err = null;

        if (IsSuccess(response.StatusCode))
            pet = TryDeserialize<Pet>(response.Content);
        else
            err = TryDeserialize<ErrorResponse>(response.Content);

        return (response, pet, err);
    }

    private static bool IsSuccess(HttpStatusCode statusCode) =>
        (int)statusCode is >= 200 and < 300;

    private static T? TryDeserialize<T>(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return default;
        try
        {
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
        catch
        {
            return default;
        }
    }
}