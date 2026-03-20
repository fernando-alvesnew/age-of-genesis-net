using RecruiterApi.Domain.Users;

namespace RecruiterApi.Application.Auth;

public interface IUserRepository
{
    Task<User?> FindByLoginOrEmailAsync(string login, CancellationToken ct);
    Task UpdateLastIpAsync(long userId, string ip, CancellationToken ct);
}

public interface ITokenService
{
    string Generate(long userId, string login, string userType);
}

public record LoginRequest(string Login, string Password);
public record LoginResponse(long UserId, string Token);

public class LoginService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public LoginService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> ExecuteAsync(LoginRequest request, string ip, CancellationToken ct)
    {
        var user = await _userRepository.FindByLoginOrEmailAsync(request.Login.Trim(), ct);
        if (user is null)
            throw new UnauthorizedAccessException("invalid credentials");

        var validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
        if (!validPassword)
            throw new UnauthorizedAccessException("invalid credentials");

        if (user.IsBanned)
            throw new InvalidOperationException("account banned");

        await _userRepository.UpdateLastIpAsync(user.Id, ip, ct);
        var token = _tokenService.Generate(user.Id, user.Login, user.UserType);

        return new LoginResponse(user.Id, token);
    }
}
