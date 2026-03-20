using Dapper;
using RecruiterApi.Application.Payments;
using RecruiterApi.Domain.Payments;

namespace RecruiterApi.Infrastructure.Persistence;

public class TransactionRepository : ITransactionRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public TransactionRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task UpsertByReferenceIdAsync(PaymentTransaction transaction, CancellationToken ct)
    {
        const string sql = """
            IF EXISTS (SELECT 1 FROM pagseguro_credit_card WHERE reference_id = @ReferenceId)
            BEGIN
                UPDATE pagseguro_credit_card
                SET
                    payment_id = @PaymentId,
                    amount = @Amount,
                    status = @Status,
                    description = @Description,
                    updated_at = GETUTCDATE()
                WHERE reference_id = @ReferenceId;
            END
            ELSE
            BEGIN
                INSERT INTO pagseguro_credit_card
                    (store_carts_id, users_id, payment_id, reference_id, amount, status, description, created_at, updated_at)
                VALUES
                    (@StoreCartId, @UserId, @PaymentId, @ReferenceId, @Amount, @Status, @Description, GETUTCDATE(), GETUTCDATE());
            END
            """;

        await using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(new CommandDefinition(sql, transaction, cancellationToken: ct));
    }
}
