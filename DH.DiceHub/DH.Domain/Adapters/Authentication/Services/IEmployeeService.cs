using DH.Domain.Adapters.Authentication.Models;

namespace DH.Domain.Adapters.Authentication.Services;

public interface IEmployeeService
{
    Task<EmployeeModel?> GetEmployeeId(string id, CancellationToken cancellationToken);
    Task<EmployeeResult> CreateEmployee(CreateEmployeeRequest request, CancellationToken cancellationToken);
    Task CreateEmployeePassword(CreateEmployeePasswordRequest request);
    Task<EmployeeResult> UpdateEmployee(UpdateEmployeeRequest request, CancellationToken cancellationToken);
    Task DeleteEmployee(string employeeId);
}
