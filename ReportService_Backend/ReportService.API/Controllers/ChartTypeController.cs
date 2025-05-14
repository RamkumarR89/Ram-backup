using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReportService.Business.Interfaces;
using ReportService.Domain.DTOs;

namespace ReportService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartTypeController : ControllerBase
    {
        private readonly IChartTypeService _chartTypeService;

        public ChartTypeController(IChartTypeService chartTypeService)
        {
            _chartTypeService = chartTypeService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ChartTypeDto>>> GetAll()
        {
            var chartTypes = await _chartTypeService.GetAllAsync();
            return Ok(chartTypes);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ChartTypeDto>> GetById(int id)
        {
            var chartType = await _chartTypeService.GetByIdAsync(id);
            if (chartType == null)
            {
                return NotFound();
            }
            return Ok(chartType);
        }
    }
} 