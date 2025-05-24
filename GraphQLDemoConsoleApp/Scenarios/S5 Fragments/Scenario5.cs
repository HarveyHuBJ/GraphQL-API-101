using GraphQL.Types;
using GraphQL.SystemTextJson;

using GraphQLDemoConsoleApp.Scenarios.S2;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQLDemoConsoleApp 
{
    public class Scenario5
    {
        public static async Task _11QueryWithFragments()
        {
            // Alias : leftComparison rightComparison
            // Fragment : droid
            var query = @"
query {
  leftComparison: droid(id: ""123"") {
    ...comparisonFields
  }
  rightComparison: droid(id: ""456"") {
    ...comparisonFields
  }
}

fragment comparisonFields on Droid {
  id name appearsIn
}
"
            ;

            var schema = new Schema { Query = new StarWarsQuery() };
            var json = await schema.ExecuteAsync(_ =>
            {
                _.Query = query;
            });


            Console.WriteLine($"Query: {query}");
            Console.WriteLine("---------");
            Console.WriteLine(json);
        }
    }
}
