using GraphQL;
using GraphQL.Types;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQLDemoConsoleApp.Utils
{
    public class CamelCaseEnumerationGraphType<T> : EnumerationGraphType<T> where T : Enum
    {
        protected override string ChangeEnumCase(string val) => val.ToCamelCase();
    }
}
