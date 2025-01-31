namespace Library.PaginationUtils
{
    public class PaginationInfo
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;

        //Costruttore
        public PaginationInfo(int page, int pageSize, int totalRecords)
        {
            Page = page;
            PageSize = pageSize;
            TotalRecords = totalRecords;
        }
    }
}
