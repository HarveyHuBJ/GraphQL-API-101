using Newtonsoft.Json;

namespace DataPlatform.GraphQL.API
{
    public class QueryArgs
    {
        // JSON, where 条件语句
        public string? Filters { get; set; }

        // JSON, order by 语句
        public string? OrderBys { get; set; }

        // JSON, pagination
        public Pagination? Pagination { get; set; }

        public string Decorate(string query)
        {
            var result = query;
            var parser = new QueryArgsParser(this);

            if (!string.IsNullOrEmpty(this.Filters)) result += $" WHERE {parser.GetFilterString()} ";
            if (!string.IsNullOrEmpty(this.OrderBys)) result += $" ORDER BY {parser.GetOrderByString()} ";
            if (Pagination != null) result += $" {parser.GetPaginationString()} ";

            return result;
        }
    }

    public class Pagination
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 100;
    }
    public class QueryArgsParser
    {
        private static readonly int default_take_count = 100;
        private readonly QueryArgs args;
        private WhereClauseParser whereClauseParser = new WhereClauseParser();
        public QueryArgsParser(QueryArgs args)
        {
            this.args = args;
            this.whereClauseParser = new WhereClauseParser();
        }

        //eg. LIMIT {take} OFFSET {skip}
        public string GetPaginationString()
        {
            if (args == null) return string.Empty;
            if (args.Pagination == null) return string.Empty;

            var take = args.Pagination.Take;
            var skip = args.Pagination.Skip;
            return $" OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";
        }

        public string GetFilterString()
        {
            // like {"and":[{"field":"field1","operator":"eq","value":"value1"},{"or":[{"field":"field2","operator":"eq","value":"value2"},{"field":"field3","operator":"eq","value":"value3"}]}]}

            var result = this.whereClauseParser.Parse(args.Filters);
            return result;

        }

        public string GetOrderByString()
        {
            // like   [{'column':'field1', 'sort_type': 'ASC'}， {'column':'field2', 'sort_type': 'DESC'}] 
            var fields = JsonConvert.DeserializeObject<Dictionary<string, SortType>>(args.OrderBys);

            return string.Join(",", fields.Select(f => $"{f.Key} {f.Value.ToString()}"));
        }
         
    }

    public enum SortType
    {
        ASC, DESC
    }
}
