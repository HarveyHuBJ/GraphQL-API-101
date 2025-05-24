using GraphQL.Types;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQLDemoConsoleApp.Scenarios.Adwk
{
    public class AdventureWorksSchema : Schema
    {
        public AdventureWorksSchema()
        {

            //Query = provider.GetService<AdventureWorksQuery>();
            var repository = new AdventureWorksRepository();

            Query = new AdventureWorksQuery(repository);
        }

        public AdventureWorksSchema(IServiceProvider provider)
        {

            Query = (AdventureWorksQuery)provider.GetService(typeof(AdventureWorksQuery));
        }
    }
}
