using AdventureWorks.Application.Loyalty.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AdventureWorks.Api.Controllers;

/// <summary>
/// Exposes loyalty account endpoints — LOY-INFRA-001.2.
/// </summary>
[ApiController]
[Route("api/loyalty")]
public class LoyaltyController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoyaltyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// GET /api/loyalty/{customerId} — returns the loyalty account for a given customer,
    /// including the current points_balance.
    /// Returns 404 if no loyalty account exists for the customer.
    /// </summary>
    [HttpGet("{customerId:int}")]
    public async Task<ActionResult<LoyaltyAccountDto>> GetByCustomerId(
        int customerId,
        CancellationToken cancellationToken)
    {
        var account = await _mediator.Send(
            new GetLoyaltyAccountByCustomerIdQuery(customerId),
            cancellationToken);

        if (account is null)
            return NotFound();

        return Ok(account);
    }
}
