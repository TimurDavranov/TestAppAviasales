using System.ComponentModel;

namespace AS.Core.Primitives
{
    public class FilterModel
    {
        [DefaultValue(1)]
        public int Page { get; set; }

        [DefaultValue(20)]
        public int Size { get; set; }

        public string SearchText { get; set; }

        public string OrderBy { get; set; }

        public static string ToRequestKey(FilterModel filter) => $"{nameof(filter.Page)}={filter.Page};{nameof(filter.Size)}={filter.Size};{nameof(filter.SearchText)}={filter.SearchText};{nameof(filter.OrderBy)}={filter.OrderBy}";
    }
}
