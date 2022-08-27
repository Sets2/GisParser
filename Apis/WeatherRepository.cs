using GisParser.Data;
using GisParser.EFData;
using GismeteoParser.Constants;
using GisParser.Constants;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static OpenQA.Selenium.PrintOptions;

namespace GisParser.Apis;

public class WeatherRepository
{
    private UriParam uriParam;
    public PageModel pageModel { get; set; }
    public IEnumerable<Weather> weather { get; set; }

    public WeatherRepository(UriParam _uriParam)
    {
        this.uriParam = _uriParam;
    }

    public async Task<bool> GetWeather(ApplicationDbContext dbContext)
    {
        var qry = (IQueryable<Weather>)dbContext.Weather.Include(x => x.MeteorologicalStation);
        if (uriParam.paramFrom != null) qry = qry.Where(o => o.Date >= uriParam.dateFrom);
        if (uriParam.paramTo != null) qry = qry.Where(o => o.Date <= uriParam.dateTo);
        if (uriParam.StationId != 0) qry = qry.Where(o => o.MeteorologicalStationId == uriParam.StationId);
        else
            if (uriParam.paramStation != null) qry = qry.Where(o => o.MeteorologicalStation.Name == uriParam.paramStation);

        try
        {
            var pageCount = await qry.CountAsync();
            pageModel = new(pageCount, uriParam.page, CommonConstants.PGSize);
            weather = await qry.OrderBy(o=>o.Date).ThenBy(o=>o.MeteorologicalStationId).Skip((pageModel.PageNumber - 1) * CommonConstants.PGSize).Take(CommonConstants.PGSize).ToListAsync();
            return true;

        }
        catch (Exception)
        {
            return false;
        }
    }
}

//public class IndexWhetherModel
//{
//    public IEnumerable<Weather> weather { get; set; }
//    public PageModel PageViewModel { get; set; }
//}
