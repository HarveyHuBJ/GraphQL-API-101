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
    [Description("The Star Wars movies.")]
    public enum Episodes
    {
        [Description("Episode 1: The Phantom Menace")]
        [Obsolete("Optional. Sets the GraphQL DeprecationReason for this member.")]
        PHANTOMMENACE = 1,

        [Description("Episode 4: A New Hope")]
        NEWHOPE = 4,

        [Description("Episode 5: The Empire Strikes Back")]
        EMPIRE = 5,

        [Description("Episode 6: Return of the Jedi")]
        JEDI = 6
    }
    public class EpisodeEnum : EnumerationGraphType<Episodes>
    {
        public EpisodeEnum()
        {
            Name = "Episode";
            Description = "One of the films in the Star Wars Trilogy.";
        }
    }

    public class Droid
    {
        public string Id { get; set; }
        public string Name { get; set; }

        // You can use EnumerationGraphType<TEnum> to automatically generate values by providing a .NET enum for TEnum.
        public List<Episodes> AppearsIn { get; set; }

    }

    public class Character
    {
        public string Name { get; set; }
    }

    #region Schema First
    
    public class Query
    {
        // Resolver Method : This method will be called when the "hero" query is executed.
        [GraphQLMetadata("hero")]
        public Droid GetHero()
        {
            return new Droid { Id = "2", Name = "R2-D2-2" };
            //return new Droid { Id = "1", Name = "R2-D2" };
        }
    }
    #endregion


    #region Code First

    public class DroidType : ObjectGraphType<Droid>
    {
        public DroidType()
        {
            Name = "Droid";
            Description = "A mechanical creature in the Star Wars universe.";

            Field(x => x.Id).Description("The Id of the Droid.");
            Field(x => x.Name).Description("The name of the Droid.");
            Field(x => x.AppearsIn).Description("The movies which droid attended.");
        }
    }

    [GraphQLMetadata("Droid", IsTypeOf = typeof(Droid))]
    public class DroidType2
    {
        public string Id([FromSource] Droid droid) => droid.Id;
        public string Name([FromSource] Droid droid) => droid.Name;

        // these two parameters are optional
        // IResolveFieldContext provides contextual information about the field
        public Character Friend(IResolveFieldContext context, [FromSource] Droid source)
        {
            return new Character { Name = "C3-PO" };
        }
    }


    public static class StarWarsData
    {
        public static List<Droid> Droids = new List<Droid>
                  {
                    new Droid { Id = "123", Name = "R2-D2" , AppearsIn = [Episodes.EMPIRE, Episodes.NEWHOPE] },
                    new Droid { Id = "456", Name = "R0-D5", AppearsIn = [Episodes.EMPIRE, Episodes.JEDI, Episodes.NEWHOPE] }
                  };
    }
    #endregion
}
