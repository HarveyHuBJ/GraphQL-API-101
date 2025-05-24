using GraphQL.Types;
using GraphQLDemoConsoleApp.Scenarios.S2;
using GraphQL.SystemTextJson;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQLDemoConsoleApp
{
    public class Scenario3
    {
        public static async Task _05QuerySimple()
        {

            var schema = new Schema { Query = new StarWarsQuery() };

            var queries = new string[] {
    "query MyHeroQuery { hero {  id name  }   }  ",
    "query   { hero {  id name  }   }  ",
    "{hero {  id name  } }    "
 };

            foreach (var query in queries)
            {

                var json = await schema.ExecuteAsync(_ =>
                {
                    _.Query = query;
                });


                Console.WriteLine($"Query: {query}");
                Console.WriteLine("---------");
                Console.WriteLine(json);
            }
        }


        public static async Task _06QueryWithOperationName()
        {
            var schema = new Schema { Query = new StarWarsQuery() };

            var query = "query MyHeroQuery { heroes {  id name  }   }  ";
            var json = await schema.ExecuteAsync(_ =>
            {
                _.OperationName = "MyHeroQuery";  // OperationName为空可以， 但是不能与Query Operation Name 不一致， 否则出错："message": "Document does not contain an operation named \u0027__\u0027."
                _.Query = query;
            });


            Console.WriteLine($"Query: {query}");
            Console.WriteLine("---------");
            Console.WriteLine(json);

        }
    }

 
}
