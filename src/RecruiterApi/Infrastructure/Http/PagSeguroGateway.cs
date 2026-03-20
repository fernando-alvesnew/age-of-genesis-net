using System.Text;
using System.Text.Json;
using RecruiterApi.Application.Payments;

namespace RecruiterApi.Infrastructure.Http;

public class PagSeguroGateway : IPagSeguroGateway
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public PagSeguroGateway(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<PagSeguroChargeResponse> ChargeAsync(PagSeguroChargeRequest request, CancellationToken ct)
    {
        var baseUrl = (_configuration["PAGSEGURO_BASE_URL"] ?? "https://sandbox.api.pagseguro.com").TrimEnd('/');
        var token = _configuration["PAGSEGURO_TOKEN"] ?? string.Empty;
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("PAGSEGURO_TOKEN is required");

        using var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);

        var body = JsonSerializer.Serialize(request);
        using var content = new StringContent(body, Encoding.UTF8, "application/json");
        using var response = await client.PostAsync($"{baseUrl}/orders", content, ct);

        var payload = await response.Content.ReadAsStringAsync(ct);
        var result = JsonSerializer.Deserialize<PagSeguroChargeResponse>(payload, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new PagSeguroChargeResponse();

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"pagseguro status {(int)response.StatusCode}: {payload}");

        return result;
    }
}
