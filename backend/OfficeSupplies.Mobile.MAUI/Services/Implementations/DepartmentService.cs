using Microsoft.Extensions.Logging;
using OfficeSupplies.Core.Entities;
using OfficeSupplies.Mobile.MAUI.Repositories.Interfaces;
using OfficeSupplies.Mobile.MAUI.Services.Interfaces;

namespace OfficeSupplies.Mobile.MAUI.Services.Implementations;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<DepartmentService> _logger;

    public DepartmentService(
        IDepartmentRepository departmentRepository,
        ILogger<DepartmentService> logger)
    {
        _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Department>> GetActiveDepartmentsAsync()
    {
        try
        {
            _logger.LogInformation("アクティブな部署一覧の取得を開始");
            
            var departments = await _departmentRepository.GetActiveDepartmentsAsync();
            
            // ビジネスルール: アクティブフラグが true の部署のみを返す
            var activeDepartments = departments.Where(d => d.IsActive).ToList();
            
            _logger.LogInformation("アクティブな部署 {Count} 件を取得しました", activeDepartments.Count);
            
            return activeDepartments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "アクティブな部署の取得に失敗しました");
            throw new InvalidOperationException("アクティブな部署の取得に失敗しました。", ex);
        }
    }

    public async Task<Department?> GetDepartmentByIdAsync(int departmentId)
    {
        try
        {
            // ビジネスルール: IDは正の値である必要がある
            if (departmentId <= 0)
            {
                _logger.LogWarning("無効な部署ID: {DepartmentId}", departmentId);
                throw new ArgumentException("部署IDは正の値である必要があります。", nameof(departmentId));
            }

            _logger.LogInformation("部署取得開始: DepartmentId={DepartmentId}", departmentId);
            
            var department = await _departmentRepository.GetByIdAsync(departmentId);
            
            if (department == null)
            {
                _logger.LogWarning("部署が見つかりません: DepartmentId={DepartmentId}", departmentId);
                return null;
            }

            // ビジネスルール: 非アクティブな部署は返さない
            if (!department.IsActive)
            {
                _logger.LogWarning("非アクティブな部署が要求されました: DepartmentId={DepartmentId}", departmentId);
                return null;
            }

            _logger.LogInformation("部署取得完了: DepartmentId={DepartmentId}, Name={DepartmentName}", 
                department.DepartmentId, department.DepartmentName);
            
            return department;
        }
        catch (ArgumentException)
        {
            // 引数例外はそのまま再スロー
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "部署の取得に失敗しました: DepartmentId={DepartmentId}", departmentId);
            throw new InvalidOperationException($"部署(ID: {departmentId})の取得に失敗しました。", ex);
        }
    }
}