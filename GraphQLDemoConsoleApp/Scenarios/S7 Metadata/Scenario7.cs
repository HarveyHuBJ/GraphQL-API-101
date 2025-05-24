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
    public class Scenario7
    {

        public static async Task _13QueryMetadata()
        {
            var schema = new Schema { Query = new StarWarsQuery() };

            var query = "query   {     __schema { types { kind name fields { name description}}}    }  ";
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
