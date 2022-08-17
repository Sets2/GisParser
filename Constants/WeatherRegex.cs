using System.Text.RegularExpressions;

namespace GismeteoParser.Constants
{
    public static class WeatherRegex
    {
        public static readonly Regex WinterDirectionRegex = new Regex(@"([А-я]+)\s*", RegexOptions.Compiled);
        public static readonly Regex WinterSpeedRegex = new Regex(@"\-*[\d]+", RegexOptions.Compiled);
        public static readonly Regex TemperatureRegex = new Regex(@"\+*(\-*\d+)", RegexOptions.Compiled);
    }
}