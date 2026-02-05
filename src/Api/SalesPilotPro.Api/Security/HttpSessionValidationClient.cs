using System.Net.Http.Json;
using SalesPilotPro.Core.Security;

namespace SalesPilotPro.Api.Security;

public sealed class HttpSessionValidationClient : ISessionValidationClient
{
    private readonly HttpClient _http;

    public HttpSessionValidationClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<bool> IsSessionValidAsync(
        Guid tenantId,
        Guid sessionId,
        Guid actorUserId,
        CancellationToken ct)
    {
        var response = await _http.PostAsJsonAsync(
            "/internal/sessions/validate",
            new
            {
                tid = tenantId,
                sid = sessionId,
                sub = actorUserId
            },
            ct);

        if (!response.IsSuccessStatusCode)
            return false;

        var payload = await response.Content.ReadFromJsonAsync<Response>(ct);
        return payload?.Valid == true;
    }

    private sealed class Response
    {
        public bool Valid { get; set; }
    }
}
