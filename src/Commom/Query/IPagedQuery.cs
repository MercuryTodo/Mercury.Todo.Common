namespace Common.Query
{
    public interface IPagedQuery : IQuery
    {
        int Page { get; }
        int Results { get; }
        int Offset { get; }
        string OrderBy { get; }
        string SortOrder { get; }
    }
}