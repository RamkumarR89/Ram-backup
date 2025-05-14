using System.Collections.Generic;
using System.Threading.Tasks;
using ReportService.Domain.DTOs;

namespace ReportService.Business.Interfaces
{
    public interface IChartTypeService
    {
        Task<IEnumerable<ChartTypeDto>> GetAllAsync();
        Task<ChartTypeDto?> GetByIdAsync(int id);
    }
} 