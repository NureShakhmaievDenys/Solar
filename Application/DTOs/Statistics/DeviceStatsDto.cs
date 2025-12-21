namespace Application.DTOs.Statistics
{
    public class DeviceStatsDto
    {
        public DateTime Date { get; set; }

        // Суммарная энергия за день (примерный расчет из Ватт)
        public double EnergyKwh { get; set; }

        // Максимальная мощность за день (пик выработки)
        public int PeakPowerWatts { get; set; }

        // Средняя мощность (когда солнце светило)
        public double AveragePowerWatts { get; set; }

        // Сколько денег принесло это конкретное устройство
        public double MoneySaved { get; set; }
    }
}