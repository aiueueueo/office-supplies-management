using OfficeSupplies.Core.Entities;

namespace OfficeSupplies.Mobile.MAUI.Repositories.Interfaces;

public interface IDepartmentRepository : IBaseRepository<Department>
{
    Task<IEnumerable<Department>> GetActiveDepartmentsAsync();
}