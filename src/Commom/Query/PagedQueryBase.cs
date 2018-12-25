namespace Common.Query
{
    public abstract class PagedQueryBase : IPagedQuery
    {
        public PagedQueryBase()
        {
        }

        public PagedQueryBase(int page, int results, string orderBy, string sortOrder)
        {
            Page = page;
            Results = results;
            Offset = page * results;
            OrderBy = orderBy;
            SortOrder = sortOrder;
        }

        public int Page { get; set; }
        public int Results { get; set; }
        public int Offset { get; set; }
        public string OrderBy { get; set; }
        public string SortOrder { get; set; }
    }
}