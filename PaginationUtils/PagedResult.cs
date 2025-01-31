namespace Library.PaginationUtils
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public PaginationInfo Pagination { get; set; }

        //Costruttore
        public PagedResult(IEnumerable<T> items, PaginationInfo pagination)
        {
            Items = items;
            Pagination = pagination;
        }
    }
}
