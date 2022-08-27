namespace GisParser.Apis
{
    ///<summary>   
    /// Interface to get data from site
    ///</summary>
    public interface IParser
    {
        ///<summary>   
        /// Getting data by station, year and month
        ///</summary>
        ///
        /// <param name="cityCode">code from gismeteo</param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns>Stringify JSON</returns>
        public Task<string> GetData(DateTime dateFrom, DateTime dateTo, string cityCode="", bool force=false );

    }
}
