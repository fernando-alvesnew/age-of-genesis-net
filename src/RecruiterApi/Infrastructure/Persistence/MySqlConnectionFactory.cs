using Microsoft.Data.SqlClient;

namespace RecruiterApi.Infrastructure.Persistence;

public class SqlConnectionFactory
{
    private readonly IConfiguration _configuration;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public SqlConnection Create()
    {
        var connectionString = _configuration["SQLSERVER_DSN"];
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("SQLSERVER_DSN is required");

        return new SqlConnection(connectionString);
    }
}
