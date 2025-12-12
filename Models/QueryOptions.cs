using System.Linq.Expressions;
namespace WebShop1.Models
{
    public class QueryOptions<T> where T : class
    {
        public Expression<Func<T, Object>> OrderBy { get; set; } = null;
        public Expression<Func<T, bool>> Where { get; set; } = null!;
        private string[] inclodes = Array.Empty<string>();

        public string Inclodes
        {
            set => inclodes = value.Replace("", "").Split(',');
        }
        public string[] GetIncludes() => inclodes;
        public bool HasWhere=> Where != null;
        public bool HasOrderBy => OrderBy != null;


    }
}