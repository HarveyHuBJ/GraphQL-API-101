using GraphQLDemoConsoleApp.Scenarios.Adwk;

namespace GraphQLDemoConsoleApp
{
    // https://graphql.org/learn/schema/#type-language
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var index = 0;

            if (args.Any() && !int.TryParse(args[0], out index))
            {
                index = 0;
            }
            //index = 22;


            var content = new Dictionary<int, Func<Task>>
            {
                {1,  Scenario1._01HelloWorld},

                {2,  Scenario2._02Query_SchemaFirst},
                {3,  Scenario2._03Query_CodeFirst},
                {4,  Scenario2._04Query_Multi_TopLevel_SchemaFirst},

                {5,  Scenario3._05QuerySimple},
                {6,  Scenario3._06QueryWithOperationName},

                {7,  Scenario4._07QueryWithArguments_SF},
                {8,  Scenario4._08QueryWithArguments_CF},
                {9,  Scenario4._09QueryWithVariables},
                {10,  Scenario4._10QueryWithVariablesOnly},

                {11,  Scenario5._11QueryWithFragments},

                {12,  Scenario6._12QueryWithUserContext},

                {13,  Scenario7._13QueryMetadata},

                {21,  ScenarioAdwk._21QueryProductById},
                {22,  ScenarioAdwk._22QueryProducts},


            };

            if (content.ContainsKey(index))
            {
                var call = content[index];

                Console.WriteLine($"# Run {call.Method.Name}");
                await call();
            }
            else
            {
                Console.WriteLine("Contents: ");
                Console.WriteLine("--------------");
                foreach (var item in content)
                {
                    Console.WriteLine($"\t{item.Key} - \t{item.Value.Method.Name}");
                }
            }


        }
    }
}
