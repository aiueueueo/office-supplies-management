using OfficeSupplies.Core.Entities;

namespace OfficeSupplies.Mobile.MAUI.Services.Interfaces;

public interface IDepartmentService
{
    Task<IEnumerable<Department>> GetActiveDepartmentsAsync();
    Task<Department?> GetDepartmentByIdAsync(int departmentId);
}