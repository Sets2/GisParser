namespace GisParser.Data
{
    [Index("Date")]
    [Index("MeteorologicalStationId")]
    public class Weather                                                // Дневник погоды
    {
        public Weather() { }
        
        public Weather(
            int meteorologicalStationId,
            DateTime date,
            int? dailyTemperature,
            int? dailyPressure,
            int? dailyWinterSpeed,
            string? dailyWinterDirection,
            int? nightlyTemperature,
            int? nightlyPressure,
            int? nightlyWinterSpeed,
            string? nightlyWinterDirection)
        {
            MeteorologicalStationId = meteorologicalStationId;
            Date = date;

            DailyTemperature = dailyTemperature;
            DailyPressure = dailyPressure;
            DailyWinterSpeed = dailyWinterSpeed;
            DailyWinterDirection = dailyWinterDirection;

            NightlyTemperature = nightlyTemperature;
            NightlyPressure = nightlyPressure;
            NightlyWinterSpeed = nightlyWinterSpeed;
            NightlyWinterDirection = nightlyWinterDirection;
        }


        public int Id { get; set; }                                     // Первичный атрибут записи

        public int MeteorologicalStationId { get; set; }                // Навигационный атрибут Первичный ключ станции
        public MeteorologicalStation MeteorologicalStation { get; set; }// Навигационный атрибут Название 

        public DateTime Date { get; set; }                              // Дата

        public int? DailyTemperature { get; set; }                      // Температура днем
        public int? DailyPressure { get; set; }                         // Давление днем
        public int? DailyWinterSpeed { get; set; }                      // Ветер днем
        public string? DailyWinterDirection { get; set; }               // Направление ветра днем

        public int? NightlyTemperature { get; set; }                    // Температура ночью
        public int? NightlyPressure { get; set; }                       // Давление ночью
        public int? NightlyWinterSpeed { get; set; }                    // Ветер ночью
        public string? NightlyWinterDirection { get; set; }             // Направление ветра ночью

    }
}
