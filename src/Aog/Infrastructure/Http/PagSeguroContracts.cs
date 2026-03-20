using System.Text.Json.Serialization;

namespace RecruiterApi.Infrastructure.Http;

public class PagSeguroChargeRequest
{
    [JsonPropertyName("reference_id")]
    public string ReferenceId { get; set; } = string.Empty;
    [JsonPropertyName("customer")]
    public PagSeguroCustomer Customer { get; set; } = new();
    [JsonPropertyName("items")]
    public List<PagSeguroItem> Items { get; set; } = [];
    [JsonPropertyName("notification_urls")]
    public List<string> NotificationUrls { get; set; } = [];
    [JsonPropertyName("charges")]
    public List<PagSeguroChargeEnvelope> Charges { get; set; } = [];
}

public class PagSeguroCustomer
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    [JsonPropertyName("tax_id")]
    public string TaxId { get; set; } = string.Empty;
}

public class PagSeguroItem
{
    [JsonPropertyName("reference_id")]
    public string ReferenceId { get; set; } = string.Empty;
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }
    [JsonPropertyName("unit_amount")]
    public long UnitAmount { get; set; }
}

public class PagSeguroChargeEnvelope
{
    [JsonPropertyName("reference_id")]
    public string ReferenceId { get; set; } = string.Empty;
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    [JsonPropertyName("amount")]
    public PagSeguroAmount Amount { get; set; } = new();
    [JsonPropertyName("payment_method")]
    public PagSeguroPaymentMethod PaymentMethod { get; set; } = new();
}

public class PagSeguroAmount
{
    [JsonPropertyName("value")]
    public long Value { get; set; }
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "BRL";
}

public class PagSeguroPaymentMethod
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "CREDIT_CARD";
    [JsonPropertyName("installments")]
    public int Installments { get; set; } = 1;
    [JsonPropertyName("capture")]
    public bool Capture { get; set; } = true;
    [JsonPropertyName("card")]
    public PagSeguroCard Card { get; set; } = new();
    [JsonPropertyName("holder")]
    public PagSeguroHolder Holder { get; set; } = new();
}

public class PagSeguroCard
{
    [JsonPropertyName("encrypted")]
    public string Encrypted { get; set; } = string.Empty;
    [JsonPropertyName("store")]
    public bool Store { get; set; }
}

public class PagSeguroHolder
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("tax_id")]
    public string TaxId { get; set; } = string.Empty;
}

public class PagSeguroChargeResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    [JsonPropertyName("error_messages")]
    public List<Dictionary<string, string>> ErrorMessages { get; set; } = [];
    [JsonPropertyName("charges")]
    public List<PagSeguroChargeInfo> Charges { get; set; } = [];
}

public class PagSeguroChargeInfo
{
    [JsonPropertyName("reference_id")]
    public string ReferenceId { get; set; } = string.Empty;
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
    [JsonPropertyName("amount")]
    public PagSeguroAmountInfo Amount { get; set; } = new();
}

public class PagSeguroAmountInfo
{
    [JsonPropertyName("value")]
    public long Value { get; set; }
}
