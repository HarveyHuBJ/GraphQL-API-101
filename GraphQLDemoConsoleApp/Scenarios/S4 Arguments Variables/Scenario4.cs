using GraphQL;
using GraphQL.Types;
using GraphQL.SystemTextJson;

using GraphQLDemoConsoleApp.Scenarios.S2;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphQL.Introspection;
using GraphQL.Transport;

namespace GraphQLDemoConsoleApp
{
    public class Scenario4
    {

        public static async Task _07QueryWithArguments_SF()
        {
            var schema = Schema.For(@"
  type Droid {
    id: ID!
    name: String
  }

  type Query {
    droid(id: ID!): Droid
  }
", _ =>
            {
                _.Types.Include<Scenarios.S4.Query>();
            });

            var queries =
                new string[] {
                     "{ droid(id: \"123\") {  id name   }}",
                     "{ droid(id: \"456\") {  id name   }}",
                     "{ droid(id: \"789\") {  id name   }}"
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

        public static async Task _08QueryWithArguments_CF()
        {
            var queries =
                new string[] {
                    "{  droid(id: \"123\") {  id name   }}",
                    "{  droid(id: \"456\") {  id name   }}",
                    "{  droid(id: \"789\") {  id name   }}"  // not exists, should return null
                };
            var schema = new Schema { Query = new StarWarsQuery() };

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

        public static async Task _09QueryWithVariables()
        {
            var requestJson = @"
{
 ""query"": ""query DroidQuery($droidId: ID!) { droid(id: $droidId) { id name } }"",
 ""variables"": {
   ""droidId"": ""123""
 }
}
";
            var request = new GraphQLSerializer().Deserialize<GraphQLRequest>(requestJson);

            var schema = new Schema { Query = new StarWarsQuery() };
            var json = await schema.ExecuteAsync(options =>
            {
                options.Query = request.Query;
                options.OperationName = request.OperationName;
                options.Variables = request.Variables;
                options.Extensions = request.Extensions;
            });


            Console.WriteLine($"Query: {requestJson}");
            Console.WriteLine("---------");
            Console.WriteLine(json);
        }
        public static async Task _10QueryWithVariablesOnly()
        {
            var query = " query DroidQuery($droidId: ID!) { droid(id: $droidId) { id name } }";
            var variables = "{   \"droidId\": \"123\" }";

            var inputs = new GraphQLSerializer().Deserialize<Inputs>(variables);

            var schema = new Schema { Query = new StarWarsQuery() };

            var json = await schema.ExecuteAsync(options =>
            {
                options.Query = query;
                //options.OperationName = request.OperationName;
                options.Variables = inputs;
                //options.Extensions = request.Extensions;
            });

            Console.WriteLine($"Query: {query}");
            Console.WriteLine($"Variables: {variables}");
            Console.WriteLine("---------");
            Console.WriteLine(json);
        }
    }
}

namespace GraphQLDemoConsoleApp.Scenarios.S4
{
     
    public class Query
    {
        public List<Droid> _droids = StarWarsData.Droids;


        [GraphQLMetadata("droid")]
        public Droid GetDroid(string id)
        {
            return _droids.FirstOrDefault(x => x.Id == id);
        }
    }
}
