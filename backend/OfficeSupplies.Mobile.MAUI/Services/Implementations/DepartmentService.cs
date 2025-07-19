using OfficeSupplies.Core.Entities;
using OfficeSupplies.Mobile.MAUI.Repositories.Interfaces;
using OfficeSupplies.Mobile.MAUI.Services.Interfaces;

namespace OfficeSupplies.Mobile.MAUI.Services.Implementations;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentService(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<IEnumerable<Department>> GetActiveDepartmentsAsync()
    {
        return await _departmentRepository.GetActiveDepartmentsAsync();
    }

    public async Task<Department?> GetDepartmentByIdAsync(int departmentId)
    {
        return await _departmentRepository.GetByIdAsync(departmentId);
    }
}