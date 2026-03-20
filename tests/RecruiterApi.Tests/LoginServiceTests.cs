using RecruiterApi.Application.Auth;
using RecruiterApi.Domain.Users;

namespace RecruiterApi.Tests;

public class LoginServiceTests
{
    private class UserRepositoryStub : IUserRepository
    {
        public User? User { get; set; }
        public Task<User?> FindByLoginOrEmailAsync(string login, CancellationToken ct) => Task.FromResult(User);
        public Task UpdateLastIpAsync(long userId, string ip, CancellationToken ct) => Task.CompletedTask;
    }

    private class TokenServiceStub : ITokenService
    {
        public string Generate(long userId, string login, string userType) => "token123";
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var hashed = BCrypt.Net.BCrypt.HashPassword("123456");
        var userRepo = new UserRepositoryStub
        {
            User = new User
            {
                Id = 1,
                Login = "player",
                Password = hashed,
                UserType = "player"
            }
        };
        var service = new LoginService(userRepo, new TokenServiceStub());

        var result = await service.ExecuteAsync(new LoginRequest("player", "123456"), "127.0.0.1", CancellationToken.None);

        Assert.Equal(1, result.UserId);
        Assert.Equal("token123", result.Token);
    }
}
