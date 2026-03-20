using Microsoft.AspNetCore.Mvc;
using RecruiterApi.Application.Auth;

namespace RecruiterApi.Controllers;

[ApiController]
[Route("api/login")]
public class AuthController : ControllerBase
{
    private readonly LoginService _loginService;

    public AuthController(LoginService loginService)
    {
        _loginService = loginService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _loginService.ExecuteAsync(request, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown", ct);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "invalid credentials" });
        }
        catch (InvalidOperationException ex) when (ex.Message == "account banned")
        {
            return StatusCode(403, new { error = "account banned" });
        }
    }
}
