using Microsoft.EntityFrameworkCore;
using OfficeSupplies.Core.Entities;
using OfficeSupplies.Infrastructure.Data;
using OfficeSupplies.Mobile.MAUI.Repositories.Interfaces;

namespace OfficeSupplies.Mobile.MAUI.Repositories.Implementations;

public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(OfficeSuppliesContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Department>> GetActiveDepartmentsAsync()
    {
        return await _dbSet
            .Where(d => d.IsActive)
            .OrderBy(d => d.DepartmentCode)
            .ToListAsync();
    }
}