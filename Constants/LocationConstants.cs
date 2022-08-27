using GisParser.Data;

namespace GisParser.Constants
{
    public static class LocationConstants
    {
        public static string RussiaCountryValue = "156";
        public static string StavropolKrayRegionValue = "284";

        public static List<KeyValuePair<string, string>> CitiesCodes = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("5226", "Александровское"),
            new KeyValuePair<string, string>("5144", "Арзгир"),
            new KeyValuePair<string, string>("5143", "Благодарный"),
            new KeyValuePair<string, string>("5230", "Буденновск"),
            new KeyValuePair<string, string>("217909", "Воровсколесская (Черкесск, Карачаево-Черкессия)"),
            new KeyValuePair<string, string>("218150", "Вревское (Армавир, Краснодарский край)"),
            new KeyValuePair<string, string>("5229", "Георгиевск"),
            new KeyValuePair<string, string>("218112", "Горнозаводское(Прохладный, Кабардино-Балкария)"),
            new KeyValuePair<string, string>("5126", "Дивное"),
            new KeyValuePair<string, string>("5228", "Зеленокумск"),
            new KeyValuePair<string, string>("5140", "Изобильный"),
            new KeyValuePair<string, string>("5237", "Кисловодск"),
            new KeyValuePair<string, string>("5125", "Красногвардейское"),
            new KeyValuePair<string, string>("12800", "Курская (Моздок, Северная Осетия)"),
            new KeyValuePair<string, string>("5227", "Мин-Воды"),
            new KeyValuePair<string, string>("5222", "Невинномысск"),
            new KeyValuePair<string, string>("5139", "Новоалександровск"),
            new KeyValuePair<string, string>("218239", "Рощино"),
            new KeyValuePair<string, string>("5142", "Светлоград"),
            new KeyValuePair<string, string>("5141", "Ставрополь"),
            new KeyValuePair<string, string>("218109", "Тахта (Городовиковск, Калмыкия)"),
        };
    }

    public static class UserConstants
    {
        public static List<UserDto> UserLists = new List<UserDto>
        {
            new UserDto("admin", "Pp_123", "admin","",null),
            new UserDto("user", "Uu_123", "user" ,"",null)
        };
    }
}
