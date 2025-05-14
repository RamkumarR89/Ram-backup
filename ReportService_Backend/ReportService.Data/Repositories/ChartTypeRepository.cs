using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReportService.Data.Context;
using ReportService.Data.Repositories.Interfaces;
using ReportService.Domain.Entities;

namespace ReportService.Data.Repositories
{
    public class ChartTypeRepository : IChartTypeRepository
    {
        private readonly ReportServiceContext _context;

        public ChartTypeRepository(ReportServiceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChartType>> GetAllAsync()
        {
            return await _context.ChartTypes.ToListAsync();
        }

        public async Task<ChartType?> GetByIdAsync(int id)
        {
            return await _context.ChartTypes.FindAsync(id);
        }
    }
} 