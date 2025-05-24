using GraphQL;
using GraphQL.Types;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GraphQLDemoConsoleApp.Scenarios.S2 
{
    public class StarWarsQuery : ObjectGraphType
    {
        public List<Droid> _droids = StarWarsData.Droids;
        public StarWarsQuery()
        {
            Name = "Query";
            Field<ListGraphType<DroidType>>("heroes")
                    .Resolve(context =>
                    { 
                        return _droids.ToList();
                    });

            Field<DroidType>("hero")
                .Resolve(context =>
                {
                    var userContext = context.UserContext as MyGraphQLUserContext;
                    if (userContext != null && userContext.ContainsKey("major"))
                    {
                        return new Droid { Id = "999", Name = userContext["major"].ToString() };
                    }
                    return new Droid { Id = "1", Name = "R2-D2" };
                });

            Field<DroidType>("droid")
                  .Argument<IdGraphType>("id")
                  .Resolve(context =>
                  {
                      var id = context.GetArgument<string?>("id");

                      if (id != null)
                      {
                          return _droids.FirstOrDefault(x => x.Id == id);
                      }
                      return _droids.ToList();
                  });


        }
    }
 
}
