using System.ComponentModel;

namespace AS.Core.Primitives
{
    public class FilterModel
    {
        [DefaultValue(1)]
        public int Page { get; set; }

        [DefaultValue(20)]
        public int Size { get; set; }

        public string? Search { get; set; }
        public string Property { get; set; }
        public string Condition { get; set; }
    }

    public class PagginationResult<T> where T : class
    {
        public int Size { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public ICollection<T> Data { get; set; }
    }
}
