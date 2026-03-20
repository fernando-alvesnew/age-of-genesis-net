using Microsoft.AspNetCore.Mvc;
using RecruiterApi.Application.Payments;

namespace RecruiterApi.Controllers;

[ApiController]
[Route("api/payments/credit-card")]
public class PaymentsController : ControllerBase
{
    private readonly ChargeCreditCardService _chargeService;

    public PaymentsController(ChargeCreditCardService chargeService)
    {
        _chargeService = chargeService;
    }

    [HttpPost]
    public async Task<IActionResult> ChargeCreditCard([FromBody] CreditCardChargeRequest request, CancellationToken ct)
    {
        try
        {
            var result = await _chargeService.ExecuteAsync(request, ct);
            return StatusCode(201, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
