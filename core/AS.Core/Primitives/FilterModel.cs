using System.ComponentModel;

namespace AS.Core.Primitives
{
    public class FilterModel
    {
        [DefaultValue(1)]
        public int Page { get; set; } = 1;

        [DefaultValue(20)]
        public int Size { get; set; } = 20;

        [DefaultValue("")]
        public string SearchText { get; set; } = string.Empty;

        [DefaultValue("")]
        public string OrderBy { get; set; } = string.Empty;

        public static string ToRequestKey(FilterModel filter) => $"{nameof(filter.Page)}={filter.Page};{nameof(filter.Size)}={filter.Size};{nameof(filter.SearchText)}={filter.SearchText};{nameof(filter.OrderBy)}={filter.OrderBy}";
    }
}
