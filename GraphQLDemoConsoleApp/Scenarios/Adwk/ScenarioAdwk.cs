using GraphQL;
using GraphQL.SystemTextJson;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQLDemoConsoleApp.Scenarios.Adwk
{
    public class ScenarioAdwk
    {
        public static async Task _21QueryProductById()
        {

            var schema = new AdventureWorksSchema();

            var query = "query { product(id: \"680\" ) { productID  name  listPrice }}";
            //var query = "query { products { productID, name, listPrice }}";

            var json = await schema.ExecuteAsync(_ => { _.Query = query; });


            Console.WriteLine($"Query: {query}");
            Console.WriteLine("---------");
            Console.WriteLine(json);
        }

        public static async Task _22QueryProducts()
        {

            var schema = new AdventureWorksSchema();

            var queries = 
               new string[]{
                "query { products(pageIndex: 1, pageSize:3 ) { productID  name  listPrice }}"
                ,"query { products(pageIndex: 2, pageSize:5 )  { productID  name  listPrice }}"
            }; 

            foreach (var query in queries)
            {

                var json = await schema.ExecuteAsync(_ => { _.Query = query; });


                Console.WriteLine($"Query: {query}");
                Console.WriteLine("---------");
                Console.WriteLine(json);
            }
        }
    }
}
