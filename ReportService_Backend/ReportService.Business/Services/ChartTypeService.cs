using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ReportService.Business.Interfaces;
using ReportService.Data.Repositories.Interfaces;
using ReportService.Domain.DTOs;
using ReportService.Domain.Entities;

namespace ReportService.Business.Services
{
    public class ChartTypeService : IChartTypeService
    {
        private readonly IChartTypeRepository _chartTypeRepository;
        private readonly IMapper _mapper;

        public ChartTypeService(IChartTypeRepository chartTypeRepository, IMapper mapper)
        {
            _chartTypeRepository = chartTypeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ChartTypeDto>> GetAllAsync()
        {
            var chartTypes = await _chartTypeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ChartTypeDto>>(chartTypes);
        }

        public async Task<ChartTypeDto?> GetByIdAsync(int id)
        {
            var chartType = await _chartTypeRepository.GetByIdAsync(id);
            return chartType != null ? _mapper.Map<ChartTypeDto>(chartType) : null;
        }
    }
} 