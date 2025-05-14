using System.Collections.Generic;
using System.Threading.Tasks;
using ReportService.Domain.Entities;

namespace ReportService.Data.Repositories.Interfaces
{
    public interface IChartTypeRepository
    {
        Task<IEnumerable<ChartType>> GetAllAsync();
        Task<ChartType?> GetByIdAsync(int id);
    }
} 