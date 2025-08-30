using System.Text.Json;

namespace PetStore.Client.Http;

public sealed class LoggingHandler : DelegatingHandler
{
    private readonly Action<string> _log;
    private readonly JsonSerializerOptions _json;

    public LoggingHandler(HttpMessageHandler inner, Action<string> log)
        : base(inner)
    {
        _log = log;
        _json = Serialization.Default;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        await LogRequest(request, ct);
        var response = await base.SendAsync(request, ct);
        await LogResponse(response, ct);
        return response;
    }

    private async Task LogRequest(HttpRequestMessage req, CancellationToken ct)
    {
        _log($"[RQ] {req.Method} {req.RequestUri}");
        if (req.Headers.Any())
            _log("[RQ HEADERS]\n" + string.Join("\n", req.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}")));

        if (req.Content != null)
        {
            var body = await req.Content.ReadAsStringAsync(ct);
            if (!string.IsNullOrWhiteSpace(body))
                _log($"[RQ BODY] {PrettyJson(body)}");
        }
    }

    private async Task LogResponse(HttpResponseMessage res, CancellationToken ct)
    {
        _log($"[RS] {(int)res.StatusCode} {res.ReasonPhrase}");
        if (res.Headers.Any())
            _log("[RS HEADERS]\n" + string.Join("\n", res.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}")));
        if (res.Content != null)
        {
            var body = await res.Content.ReadAsStringAsync(ct);
            if (!string.IsNullOrWhiteSpace(body))
                _log($"[RS BODY] {PrettyJson(body)}");
        }
    }

    private string PrettyJson(string input)
    {
        try
        {
            using var doc = JsonDocument.Parse(input);
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions(_json) { WriteIndented = true });
        }
        catch { return input; }
    }
}
