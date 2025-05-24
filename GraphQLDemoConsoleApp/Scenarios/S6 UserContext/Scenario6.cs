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
    public class Scenario6
    {

        public static async Task _12QueryWithUserContext()
        {
            var schema = new Schema { Query = new StarWarsQuery() };

            var query = "query MyHeroQuery { hero {  id name  }   }  ";
            var json = await schema.ExecuteAsync(_ =>
            {
                _.Query = query;
                _.UserContext = new MyGraphQLUserContext();
            });


            Console.WriteLine($"Query: {query}");
            Console.WriteLine("---------");
            Console.WriteLine(json);

        }
    }
}
