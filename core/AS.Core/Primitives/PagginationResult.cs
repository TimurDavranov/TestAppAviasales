namespace AS.Core.Primitives;

public class PagginationResult<T> where T : class
{
    public int Size { get; set; }
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public T Data { get; set; }
}