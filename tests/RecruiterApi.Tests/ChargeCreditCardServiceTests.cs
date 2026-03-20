using RecruiterApi.Application.Payments;
using RecruiterApi.Domain.Payments;
using RecruiterApi.Infrastructure.Http;

namespace RecruiterApi.Tests;

public class ChargeCreditCardServiceTests
{
    private class GatewayStub : IPagSeguroGateway
    {
        public Task<PagSeguroChargeResponse> ChargeAsync(PagSeguroChargeRequest request, CancellationToken ct)
        {
            return Task.FromResult(new PagSeguroChargeResponse
            {
                Id = "order_1",
                Charges =
                [
                    new PagSeguroChargeInfo
                    {
                        ReferenceId = "ref_1",
                        Status = PaymentConstants.StatusPaid,
                        Amount = new PagSeguroAmountInfo { Value = 1000 }
                    }
                ]
            });
        }
    }

    private class TransactionRepositoryStub : ITransactionRepository
    {
        public PaymentTransaction? LastTx { get; private set; }
        public Task UpsertByReferenceIdAsync(PaymentTransaction transaction, CancellationToken ct)
        {
            LastTx = transaction;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task ExecuteAsync_ShouldMapPaidToWaiting()
    {
        var txRepo = new TransactionRepositoryStub();
        var service = new ChargeCreditCardService(new GatewayStub(), txRepo);

        var result = await service.ExecuteAsync(new CreditCardChargeRequest
        {
            UserId = 1,
            StoreCartId = 10,
            CreditCardHolder = "John Doe",
            CpfForCard = "12345678910",
            EncryptedCard = "encrypted_data",
            Amount = 1000,
            CustomerEmail = "john@example.com",
            NotificationUrl = "https://example.com/api/payment-notification",
            Items =
            [
                new PaymentItem { ReferenceId = "item_1", Name = "Item A", Quantity = 1, UnitAmount = 1000 }
            ]
        }, CancellationToken.None);

        Assert.Equal(PaymentConstants.StatusWaiting, result.Status);
    }
}
