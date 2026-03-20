using RecruiterApi.Domain.Payments;

namespace RecruiterApi.Application.Payments;

public interface ITransactionRepository
{
    Task UpsertByReferenceIdAsync(PaymentTransaction transaction, CancellationToken ct);
}

public interface IPagSeguroGateway
{
    Task<PagSeguroChargeResponse> ChargeAsync(PagSeguroChargeRequest request, CancellationToken ct);
}

public class CreditCardChargeRequest
{
    public long UserId { get; set; }
    public long StoreCartId { get; set; }
    public string CreditCardHolder { get; set; } = string.Empty;
    public string CpfForCard { get; set; } = string.Empty;
    public string EncryptedCard { get; set; } = string.Empty;
    public long Amount { get; set; }
    public string? Description { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public string NotificationUrl { get; set; } = string.Empty;
    public List<PaymentItem> Items { get; set; } = [];
}

public class ChargeCreditCardService
{
    private readonly IPagSeguroGateway _pagSeguroGateway;
    private readonly ITransactionRepository _transactionRepository;

    public ChargeCreditCardService(IPagSeguroGateway pagSeguroGateway, ITransactionRepository transactionRepository)
    {
        _pagSeguroGateway = pagSeguroGateway;
        _transactionRepository = transactionRepository;
    }

    public async Task<PaymentTransaction> ExecuteAsync(CreditCardChargeRequest request, CancellationToken ct)
    {
        ValidateRequest(request);

        var referenceId = GenerateReferenceId();
        var payload = BuildChargePayload(request, referenceId);
        var charge = await ExecuteChargeAsync(payload, ct);
        var transaction = BuildTransaction(request, charge);

        await _transactionRepository.UpsertByReferenceIdAsync(transaction, ct);
        return transaction;
    }

    private static void ValidateRequest(CreditCardChargeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.EncryptedCard))
            throw new ArgumentException("encrypted_card is required");
    }

    private static string GenerateReferenceId()
    {
        return Guid.NewGuid().ToString("N");
    }

    private static PagSeguroChargeRequest BuildChargePayload(CreditCardChargeRequest request, string referenceId)
    {
        return new PagSeguroChargeRequest
        {
            ReferenceId = referenceId,
            Customer = new PagSeguroCustomer
            {
                Name = request.CreditCardHolder,
                Email = request.CustomerEmail,
                TaxId = request.CpfForCard
            },
            Items = request.Items.Select(MapItem).ToList(),
            NotificationUrls = [request.NotificationUrl],
            Charges = [BuildChargeEnvelope(request, referenceId)]
        };
    }

    private static PagSeguroItem MapItem(PaymentItem item)
    {
        return new PagSeguroItem
        {
            ReferenceId = item.ReferenceId,
            Name = item.Name,
            Quantity = item.Quantity,
            UnitAmount = item.UnitAmount
        };
    }

    private static PagSeguroChargeEnvelope BuildChargeEnvelope(CreditCardChargeRequest request, string referenceId)
    {
        return new PagSeguroChargeEnvelope
        {
            ReferenceId = referenceId,
            Description = request.Description ?? string.Empty,
            Amount = new PagSeguroAmount { Value = request.Amount, Currency = "BRL" },
            PaymentMethod = new PagSeguroPaymentMethod
            {
                Type = PaymentConstants.MethodCreditCard,
                Installments = 1,
                Capture = true,
                Card = new PagSeguroCard { Encrypted = request.EncryptedCard, Store = false },
                Holder = new PagSeguroHolder { Name = request.CreditCardHolder, TaxId = request.CpfForCard }
            }
        };
    }

    private async Task<PagSeguroChargeResponse> ExecuteChargeAsync(PagSeguroChargeRequest payload, CancellationToken ct)
    {
        var charge = await _pagSeguroGateway.ChargeAsync(payload, ct);
        if (charge.ErrorMessages.Count > 0)
            throw new InvalidOperationException("pagseguro returned charge errors");

        if (charge.Charges.Count == 0)
            throw new InvalidOperationException("pagseguro returned no charges");

        return charge;
    }

    private static PaymentTransaction BuildTransaction(CreditCardChargeRequest request, PagSeguroChargeResponse charge)
    {
        var firstCharge = charge.Charges.First();

        return new PaymentTransaction
        {
            UserId = request.UserId,
            StoreCartId = request.StoreCartId,
            PaymentId = charge.Id,
            ReferenceId = firstCharge.ReferenceId,
            Amount = firstCharge.Amount.Value,
            Status = NormalizeStatus(firstCharge.Status),
            Description = request.Description
        };
    }

    private static string NormalizeStatus(string apiStatus)
    {
        return apiStatus == PaymentConstants.StatusPaid
            ? PaymentConstants.StatusWaiting
            : apiStatus;
    }
}
