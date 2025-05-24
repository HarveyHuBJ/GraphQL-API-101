using GraphQL.SystemTextJson;
using GraphQL.Types;

using GraphQLDemoConsoleApp.Scenarios.S2;

namespace GraphQLDemoConsoleApp
{
    // https://graphql.org/learn/schema/#type-language
    public class Scenario2
    {



        public static async Task _02Query_SchemaFirst()
        {
            ISchema schema = Schema.For(@"
  type Droid {
    id: String!
    name: String!
  }

  type Query {
    hero: Droid
  }
", _ =>
            {
                _.Types.Include<Query>();
            });

            string query = "query { hero { id name } }";

            var json = await schema.ExecuteAsync(_ =>
                {
                    _.Query = query;
                });


            Console.WriteLine($"Query: {query}");
            Console.WriteLine("---------");
            Console.WriteLine(json);

        }


        public static async Task _03Query_CodeFirst()
        {
            var schema = new Schema { Query = new StarWarsQuery() };

            string query = "{ hero { id name } }";
            var json = await schema.ExecuteAsync(_ =>
            {
                _.Query = query;
            });

            Console.WriteLine($"Query: {query}");
            Console.WriteLine("---------");
            Console.WriteLine(json);
        }


        public static async Task _04Query_Multi_TopLevel_SchemaFirst()
        {
            var schema = Schema.For(@"
          type Droid {
            id: String!
            name: String!
            friend: Character
          }

          type Character {
            name: String!
          }

          type Query {
            hero: Droid
          }
        ", builder =>
            {
                builder.Types.Include<DroidType2>();
                builder.Types.Include<Query>();
            });

            string query = "{ hero { id name friend { name } } }";
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
