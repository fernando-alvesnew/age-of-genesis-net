using Dapper;
using RecruiterApi.Application.Auth;
using RecruiterApi.Domain.Users;

namespace RecruiterApi.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public UserRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> FindByLoginOrEmailAsync(string login, CancellationToken ct)
    {
        const string sql = """
            SELECT TOP (1)
                id, login, email, password, user_type AS UserType, is_banned AS IsBanned, last_ip AS LastIp
            FROM users
            WHERE login = @login OR email = @login;
            """;

        await using var connection = _connectionFactory.Create();
        return await connection.QueryFirstOrDefaultAsync<User>(new CommandDefinition(sql, new { login }, cancellationToken: ct));
    }

    public async Task UpdateLastIpAsync(long userId, string ip, CancellationToken ct)
    {
        const string sql = "UPDATE users SET last_ip = @ip WHERE id = @userId;";
        await using var connection = _connectionFactory.Create();
        await connection.ExecuteAsync(new CommandDefinition(sql, new { ip, userId }, cancellationToken: ct));
    }
}
