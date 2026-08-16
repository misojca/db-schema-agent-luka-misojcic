using MediatR;

namespace AdventureWorks.Application.Employees.Queries;

/// <summary>
/// Query: retrieve all employees from humanresources.employee.
/// Returns a list of <see cref="EmployeeDto"/>.
/// </summary>
public record GetAllEmployeesQuery : IRequest<List<EmployeeDto>>;
