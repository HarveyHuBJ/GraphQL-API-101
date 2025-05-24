# S6 UserContext

在本例中， 介绍了在 **GraphQL.NET** 中使用 **UserContext** 来传递上下文信息。`UserContext` 是一个可以在解析器中使用的对象，用于存储请求相关的数据，例如用户身份验证信息、配置参数等。



执行代码

~~~c#
public static async Task _12QueryWithUserContext()
{
    var schema = new Schema { Query = new StarWarsQuery() };

    var query = "query MyHeroQuery { hero {  id name  }   }  ";
    var json = await schema.ExecuteAsync(_ =>
    {
        _.Query = query;
        _.UserContext = new MyGraphQLUserContext();
    });


    Console.WriteLine($"Query: {query}");
    Console.WriteLine("---------");
    Console.WriteLine(json);

}
~~~



其中`MyGraphQLUserContext` 本质上是一个字典对象，可以由用户准备相关的上下文信息。

~~~c#
public class MyGraphQLUserContext : Dictionary<string, object?>
{
    public MyGraphQLUserContext()
    {
        this["major"] = "Sky Walker";
    }
}
~~~



解析时利用上下文信息：

~~~c#
public class StarWarsQuery : ObjectGraphType
{
    public List<Droid> _droids = StarWarsData.Droids;

    public StarWarsQuery()
    {
        Name = "Query";

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
    }
}
~~~

- **`Resolve` 方法**：定义了字段的解析逻辑。
  - **`context.UserContext`**：获取当前请求的上下文信息。
  - **`MyGraphQLUserContext`**：假设 `UserContext` 是一个自定义的上下文类，存储了请求相关的数据。
  - 如果 `UserContext` 中包含键 `"major"`，则返回一个自定义的 `Droid` 对象，其 `Id` 为 `"999"`，`Name` 为 `userContext["major"].ToString()`。
  - 如果没有找到 `"major"`，则返回一个默认的 `Droid` 对象，其 `Id` 为 `"1"`，`Name` 为 `"R2-D2"`。

运行示例

~~~cmd
dotnet run 12
~~~



运行结果

~~~bash
# Run _12QueryWithUserContext
Query: query MyHeroQuery { hero {  id name  }   }  
---------
{
  "data": {
    "hero": {
      "id": "999",
      "name": "Sky Walker"
    }
  }
}
~~~

