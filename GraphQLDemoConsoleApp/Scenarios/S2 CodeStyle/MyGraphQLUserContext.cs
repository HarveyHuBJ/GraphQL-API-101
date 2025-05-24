using GraphQL.Types;
using GraphQLDemoConsoleApp.Scenarios.S2;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQLDemoConsoleApp 
{
    public class MyGraphQLUserContext : Dictionary<string, object?>
    {
        public MyGraphQLUserContext()
        {
            this["major"] = "Sky Walker";
        }
    }
}
