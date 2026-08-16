using AdventureWorks.Application.Employees.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AdventureWorks.Api.Controllers;

/// <summary>
/// Exposes employee endpoints — issue #17.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// GET /api/employees — returns all employees from humanresources.employee.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<EmployeeDto>>> GetAll(CancellationToken cancellationToken)
    {
        var employees = await _mediator.Send(new GetAllEmployeesQuery(), cancellationToken);
        return Ok(employees);
    }
}
