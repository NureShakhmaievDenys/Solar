using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Statistics
{
    public class StatisticsResponseDto
    {
        public DailyStatisticsDto Totals { get; set; } = null!;

        public List<DailyStatisticsDto> DailyStats { get; set; } = new List<DailyStatisticsDto>();
    }
}
