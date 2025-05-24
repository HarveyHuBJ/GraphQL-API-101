using GraphQL;
using GraphQL.SystemTextJson;
using GraphQL.Types;

namespace GraphQLDemoConsoleApp
{
    public class Scenario1
    {
        public static async Task _01HelloWorld()
        {
            var schema = Schema.For(@"
              type Query {
                hello: String
              }
            ");
            var serializer = new GraphQLSerializer();

            var query = "{ hello }";
            var json = await schema.ExecuteAsync(serializer, _ =>
            {
                _.Query = query;
                _.Root = new { Hello = "Hello World!" };
            });

            Console.WriteLine($"Query: {query}");
            Console.WriteLine("---------");
            Console.WriteLine(json);
        }
    }
}
