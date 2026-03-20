namespace RecruiterApi.Domain.Payments;

public static class PaymentConstants
{
    public const string MethodCreditCard = "CREDIT_CARD";
    public const string StatusPaid = "PAID";
    public const string StatusWaiting = "WAITING";
}

public class PaymentItem
{
    public string ReferenceId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public long UnitAmount { get; set; }
}

public class PaymentTransaction
{
    public long Id { get; set; }
    public long StoreCartId { get; set; }
    public long UserId { get; set; }
    public string PaymentId { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty;
    public long Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
}
