namespace GisParser.Data
{
    public class MeteorologicalStation
    {
        public MeteorologicalStation(int id,string name)
        {
            Id = id;
            Name = name;
        }
        public int Id { get; set; }         // Первичный ключ
        public string Name { get; set; }    // Наименование станции по населенному пункту

        public ICollection<MeteorologicalStation> Weather { get; set; } // Навигационный атрибут по погоде

    }
}
