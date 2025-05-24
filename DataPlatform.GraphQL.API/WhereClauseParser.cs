using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DataPlatform.GraphQL.API
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using System;
    using System.Collections.Generic;


    
    public class WhereClauseParser
    {
        public string Parse(string jsonExpression)
        {
            var jObject = JObject.Parse(jsonExpression);
            return Visit(jObject);
        }
        /// <summary>
        ///  eg: {"and":[ "field1": {"eq":"value"} , "or":["field2": {"eq":"value2"}, "field3": {"eq":"value3"}]]}
        ///  ==> ((field1 = 'value1') AND ((field2 = 'value2') OR (field3 = 'value3')))
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private string Visit(JToken token)
        {
            if (token is JObject obj)
            {
                foreach (var property in obj.Properties())
                {
                    var key = property.Name;
                    var value = property.Value;

                    // 检查是否是 and/or 等逻辑操作符
                    if (key.ToLower() == "and" || key.ToLower() == "or")
                    {
                        var logicalOperator = key.ToUpper();
                        var conditions = new List<string>();

                        if (value is JArray array)
                        {
                            foreach (var item in array)
                            {
                                conditions.Add(Visit(item));
                            }
                        }
                        else if (value is JObject subObj)
                        {
                            conditions.Add(Visit(subObj));
                        }

                        return $"({string.Join($" {logicalOperator} ", conditions)})";
                    }
                    else
                    {
                        // 处理单个字段条件，如 { "field1": { "eq": "value" } }
                        var field = key;
                        var condition = value as JObject;
                        var op = condition?.Properties().FirstOrDefault()?.Name;
                        var val = condition?[op]?.ToString();

                        if (op != null && val != null)
                        {
                            var standardOP = MapOperator(op);
                            if (standardOP == "IN")
                            {
                                var array = condition?[op].ToArray();
                                var temp_val = "'" + string.Join("','", array.Select(p => p)) + "'";
                                return $"{field} IN  ({temp_val}) ";
                            }
                            else if (standardOP == "LIKE" || standardOP == "CONTAINS")
                            {
                                return $"{field} LIKE '%{val}%'";
                            }
                            else
                            {
                                return $"{field} {standardOP} '{val}'";
                            }

                            //return $"{field} {MapOperator(op)} '{val}'";
                        }
                    }
                }
            }

            throw new ArgumentException("Invalid JSON expression");
        }

        private string MapOperator(string op)
        {
            switch (op.ToLower())
            {
                case "eq": return "=";
                case "neq": return "<>";
                case "gt": return ">";
                case "gte": return ">=";
                case "lt": return "<";
                case "lte": return "<=";
                case "like": return "LIKE";
                case "in": return "IN";
                default: throw new ArgumentException($"Unsupported operator: {op}");
            }
        }
    }
}




//public interface IFilter
//{
//}

//public class AndFilter : List<IFilter> { }
//public class OrFilter : List<IFilter> { }
//public class FilterCondition : IFilter
//{
//    public string Field { get; set; }

//    /// <summary>
//    /// op can be one of the following: eq, ne, gt, lt, ge, le, in, nin, startsWith, endsWith, contains
//    /// </summary>
//    public FilterOperator Op { get; set; }
//    public object Value { get; set; }
//}

//public enum FilterOperator
//{
//    Eq, Ne, Gt, Lt, Ge, Le, In, Nin, StartsWith, EndsWith, Contains
//}


//public class FilterGroup
//{
//    public AndFilter And { get; set; } = new AndFilter();
//    public OrFilter Or { get; set; } = new OrFilter();
//}

//public class WhereClauseParser
//{
//    private Dictionary<string, object> _parameters = new Dictionary<string, object>();

//    public Dictionary<string, object> GetParameters() => new Dictionary<string, object>(_parameters);

//    private bool useParameter { get; } = false;

//    public string Parse(FilterGroup filters)
//    {
//        _parameters.Clear();
//        // 转换为json 处理
//        var jsonExpression = JsonConvert.SerializeObject(filters);
//        var jObject = JObject.Parse(jsonExpression);
//        string whereClause = Visit(jObject);
//        return whereClause;

//    }

//    private string Visit(JToken token)
//    {
//        if (token is JObject obj)
//        {
//            // Check for logical operators: and/or
//            if (obj.TryGetValue("and", out var andToken))
//            {
//                var clauses = new List<string>();
//                foreach (var item in (JArray)andToken)
//                {
//                    string clause = Visit(item);
//                    if (!string.IsNullOrEmpty(clause))
//                        clauses.Add(clause);
//                }

//                return clauses.Count == 0 ? null : $"({string.Join(" AND ", clauses)})";
//            }
//            else if (obj.TryGetValue("or", out var orToken))
//            {
//                var clauses = new List<string>();
//                foreach (var item in (JArray)orToken)
//                {
//                    string clause = Visit(item);
//                    if (!string.IsNullOrEmpty(clause))
//                        clauses.Add(clause);
//                }

//                return clauses.Count == 0 ? null : $"({string.Join(" OR ", clauses)})";
//            }
//            else
//            {
//                // This is a field condition
//                foreach (var prop in obj.Properties())
//                {
//                    string fieldName = prop.Name;
//                    JObject condObj = prop.Value as JObject;

//                    if (condObj == null || !condObj.HasValues)
//                        continue;

//                    var conditions = new List<string>();

//                    foreach (var condProp in condObj.Properties())
//                    {
//                        string op = condProp.Name.ToLower();
//                        JToken value = condProp.Value;

//                        if (useParameter)
//                        {

//                            string paramName = $"@{fieldName}_{Guid.NewGuid().ToString("N")}";

//                            switch (op)
//                            {
//                                case "eq":
//                                    conditions.Add($"{fieldName} = {paramName}");
//                                    break;
//                                case "ne":
//                                    conditions.Add($"{fieldName} <> {paramName}");
//                                    break;
//                                case "lt":
//                                    conditions.Add($"{fieldName} < {paramName}");
//                                    break;
//                                case "le":
//                                    conditions.Add($"{fieldName} <= {paramName}");
//                                    break;
//                                case "gt":
//                                    conditions.Add($"{fieldName} > {paramName}");
//                                    break;
//                                case "ge":
//                                    conditions.Add($"{fieldName} >= {paramName}");
//                                    break;
//                                case "contains":
//                                    conditions.Add($"{fieldName} LIKE {paramName}");
//                                    break;
//                                case "in":
//                                    if (value is JArray array)
//                                    {
//                                        var inParams = new List<string>();
//                                        int i = 0;
//                                        foreach (var item in array)
//                                        {
//                                            string inParamName = $"{paramName}_item_{i++}";
//                                            inParams.Add(inParamName);
//                                            _parameters[inParamName] = item.ToObject<object>();
//                                        }
//                                        conditions.Add($"{fieldName} IN ({string.Join(", ", inParams)})");
//                                    }
//                                    else
//                                    {
//                                        throw new ArgumentException("IN operator expects an array.");
//                                    }
//                                    break;
//                                default:
//                                    throw new ArgumentException($"Unsupported operator: {op}");
//                            }

//                            _parameters[paramName] = value.Type == JTokenType.String ? value.ToString() : value.ToObject<object>();
//                        }
//                        else
//                        {
//                            switch (op)
//                            {
//                                case "eq":
//                                    conditions.Add($"{fieldName} = {value}");
//                                    break;
//                                case "ne":
//                                    conditions.Add($"{fieldName} <> {value}");
//                                    break;
//                                case "lt":
//                                    conditions.Add($"{fieldName} < {value}");
//                                    break;
//                                case "gt":
//                                    conditions.Add($"{fieldName} > {value}");
//                                    break;
//                                case "le":
//                                    conditions.Add($"{fieldName} <= {value}");
//                                    break;
//                                case "ge":
//                                    conditions.Add($"{fieldName} >= {value}");
//                                    break;
//                                case "in":
//                                    conditions.Add($"{fieldName} IN ('{string.Join("','", value)}')");
//                                    break;
//                                case "nin":
//                                    conditions.Add($"{fieldName} NOT IN ('{string.Join("','", value)}')");
//                                    break;
//                                case "startsWith":
//                                    conditions.Add($"{fieldName} LIKE '{value}%'");
//                                    break;
//                                case "endsWith":
//                                    conditions.Add($"{fieldName} LIKE '%{value}'");
//                                    break;
//                                case "contains":
//                                    conditions.Add($"{fieldName} LIKE '%{value}%'");
//                                    break;
//                                default:
//                                    throw new ArgumentException($"Unsupported operator: {op}");

//                            }
//                        }
//                    }

//                    if (conditions.Count == 1)
//                        return conditions[0];
//                    else if (conditions.Count > 0)
//                        return $"({string.Join(" AND ", conditions)})";
//                }
//            }
//        }
//        else if (token is JArray arrayToken)
//        {
//            var clauses = new List<string>();
//            foreach (var item in arrayToken)
//            {
//                string clause = Visit(item);
//                if (!string.IsNullOrEmpty(clause))
//                    clauses.Add(clause);
//            }

//            return clauses.Count == 0 ? null : $"({string.Join(" OR ", clauses)})";
//        }

//        return null;
//    }

//}