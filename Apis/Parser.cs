using GisParser.Data;
using GisParser.EFData;
using OpenQA.Selenium;
using GisParser.Constants;
using GismeteoParser.Constants;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace GisParser.Apis
{
    public class Parser : IParser
    {
        private const string GismeteoBaseUrl = "https://www.gismeteo.ru/diary/";
        private const int minYear = 2016;
        //private DateTime _dateFrom;
        //private DateTime _dateTo;

        private IWebDriver _browser;
        private ApplicationDbContext _dbContext;

        public Parser(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext; 
            _browser=new OpenQA.Selenium.Chrome.ChromeDriver();
        }
        private int GetTemperatureValue(string temperature)
        {
            return int.Parse(WeatherRegex.TemperatureRegex.Match(temperature).Groups[1].Value);
        }
        private string GetWinterDirection(string winter)
        {
            return WeatherRegex.WinterDirectionRegex.Match(winter).Groups[1].Value;
        }
//        private async Task<Weather> NotInBase(DateTime d)
//        {
//            var res = await _dbContext.Weather.Where(o => o.Date == d).FirstOrDefaultAsync();
//            var res = await _dbContext.Weather.Select(o => o.Date).FirstOrDefaultAsync(o => o.Date == d);
//            return res;
//        }

        public async Task<string> GetData(DateTime dateFrom, DateTime dateTo, string cityCode = "", bool force = false)
        {
            /// varible for validate inpur parameter
            DateTime minDate= new DateTime(minYear, 01, 01); 
            DateTime maxDate= DateTime.Now.AddDays(-1);

            List<MeteorologicalStation> meteorologicalStations=null;

            /// If cityCode Empty full List MeteoroslogicalStation from table base
            /// else from this entry table base
            /// if this entry absent use full List
            if (cityCode != "")
            {
                var Result = await _dbContext.MeteorologicalStation.FindAsync(int.Parse(cityCode));
                if (Result == null) cityCode = "";
                else meteorologicalStations = new List<MeteorologicalStation> { Result };
            }
            if (cityCode == "") meteorologicalStations = _dbContext.MeteorologicalStation.ToList();

            if (dateFrom < minDate) dateFrom = minDate;
            if (dateTo > maxDate) dateTo = maxDate;
            if (dateFrom>dateTo) dateFrom = dateTo;

            foreach (var ms in meteorologicalStations)
            {
                DateTime dateCurrent = dateFrom;
                while (dateCurrent <= dateTo)
                {
                    _browser.Navigate().GoToUrl(GismeteoBaseUrl + ms.Id+"/"+ dateCurrent.Year+"/" + dateCurrent.Month+"/");
                    Delay(1000);
                    //processing the table
                    //Create table element
                    IWebElement tableElement = _browser.FindElement(By.TagName("table"));
                    //Get second table body
                    IWebElement tableBodySecond = tableElement.FindElements(By.TagName("tbody"))[1];
                    //Get list raw
                    IList<IWebElement> tableRow = tableBodySecond.FindElements(By.TagName("tr"));
                    //Get list columns from row
                    IList<IWebElement> rowTD;
                    foreach(IWebElement row in tableRow)
                    {
                        rowTD = row.FindElements(By.TagName("td"));
                        DateTime date = new DateTime(dateCurrent.Year, dateCurrent.Month, int.Parse(rowTD[0].Text));
                        /// If date in the requested range and record absent or parameter force=true
                        if ((date >= dateCurrent) & (date <= dateTo))
                        {
                            Weather tmpWeather = await _dbContext.Weather.
                                Where(o => o.Date == date & o.MeteorologicalStationId==ms.Id).FirstOrDefaultAsync();
                            if (force | (tmpWeather == null))
                            {
                                #region daily weather

                                int? dailyTemperature = null;
                                try { dailyTemperature = GetTemperatureValue(rowTD[1].Text); }
                                catch { };

                                int? dailyPressure = null;
                                try { dailyPressure = int.Parse(rowTD[2].Text); }
                                catch { };

                                int? dailyWinterSpeed = null;
                                string? dailyWinterDirection = null;
                                try
                                {
                                    IWebElement dailyWinterColumnSpanElement = rowTD[5].FindElement(By.TagName("span"));
                                    string dailyWinter = dailyWinterColumnSpanElement.Text;
                                    dailyWinterDirection = GetWinterDirection(dailyWinter);
                                    dailyWinterSpeed = dailyWinterDirection == "Ш" ? 0 : GetTemperatureValue(dailyWinter);
                                }
                                catch { };
                                #endregion

                                #region nightly weather

                                int? nightlyTemperature = null;
                                try { nightlyTemperature = GetTemperatureValue(rowTD[6].Text); }
                                catch { };

                                int? nightlyPressure = null;
                                try { nightlyPressure = int.Parse(rowTD[7].Text); }
                                catch { };

                                int? nightlyWinterSpeed = null;
                                string? nightlyWinterDirection = null;
                                try
                                {
                                    IWebElement nightlyWinterColumnSpanElement = rowTD[10].FindElement(By.TagName("span"));
                                    string nightlyWinter = nightlyWinterColumnSpanElement.Text;
                                    nightlyWinterDirection = GetWinterDirection(nightlyWinter);
                                    nightlyWinterSpeed = nightlyWinterDirection == "Ш" ? 0 : GetTemperatureValue(nightlyWinter);
                                }
                                catch { };
                                #endregion

                                bool existWeather=true;
                                if (tmpWeather == null)
                                {
                                    tmpWeather = new Weather();
                                    tmpWeather.MeteorologicalStationId = ms.Id;
                                    tmpWeather.Date = date;
                                    existWeather = false;
                                }
                                tmpWeather.DailyTemperature= dailyTemperature;
                                tmpWeather.DailyPressure= dailyPressure;
                                tmpWeather.DailyWinterSpeed= dailyWinterSpeed;
                                tmpWeather.DailyWinterDirection= dailyWinterDirection;
                                tmpWeather.NightlyTemperature= nightlyTemperature;
                                tmpWeather.NightlyPressure= nightlyPressure;
                                tmpWeather.NightlyWinterSpeed= nightlyWinterSpeed;
                                tmpWeather.NightlyWinterDirection= nightlyWinterDirection;
                                
                                //var r= JsonSerializer.Serialize<Weather>(tmpWeather);
                                //Console.WriteLine(r);
                                if (! existWeather) await _dbContext.Weather.AddAsync(tmpWeather);
                                await _dbContext.SaveChangesAsync();
                            }
                        }
                        else break;
                    }
                    dateCurrent = dateCurrent.AddMonths(1);
                }
            }
            return "";
        }

        private void Delay(int msecond)
        {
            _browser.Manage().Timeouts().ImplicitWait=TimeSpan.FromMilliseconds(msecond);
        }
    }
}
