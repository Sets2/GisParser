using System.Globalization;

namespace GisParser.Apis
{
    public class PageModel
    {
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }

        public PageModel(int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        public bool HasPreviousPage()
        {
                return (PageNumber > 1);
        }

        public bool HasNextPage()
        {
                return (PageNumber < TotalPages);
        }
    }

    public class UriParam
    {
        public DateTime dateFrom;
        public string? paramFrom;
        public DateTime dateTo;
        public string? paramTo;
        public string? paramStationId;
        public int StationId = 0;
        public string? paramStation;
        public string? paramPage;
        public int page = 0;
    }


}

