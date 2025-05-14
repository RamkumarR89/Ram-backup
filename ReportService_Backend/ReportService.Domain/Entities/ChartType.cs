using System;

namespace ReportService.Domain.Entities
{
    public class ChartType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public ICollection<ChartConfiguration> ChartConfigurations { get; set; } = new List<ChartConfiguration>();
    }
}
