using AdventureWorks.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorks.Application.Employees.Queries;

/// <summary>
/// Handles <see cref="GetAllEmployeesQuery"/> by querying the database
/// and projecting each row into an <see cref="EmployeeDto"/>.
/// </summary>
public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, List<EmployeeDto>>
{
    private readonly IApplicationDbContext _db;

    public GetAllEmployeesQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<EmployeeDto>> Handle(
        GetAllEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        return await _db.Employees
            .AsNoTracking()
            .Select(e => new EmployeeDto(
                e.BusinessEntityId,
                e.JobTitle,
                e.HireDate,
                e.CurrentPayRate,
                e.CurrentPayFrequency,
                e.PayChangeDate))
            .ToListAsync(cancellationToken);
    }
}
