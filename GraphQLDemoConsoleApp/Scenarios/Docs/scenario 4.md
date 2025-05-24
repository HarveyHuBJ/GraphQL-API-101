# S4 Query(2) - 参数与变量



## 1. 实现Query参数， Schema First示例

可以定义Query所需的参数， 这样可以根据提供的不同参数返回不同的结果。 



### 准备数据

~~~c#
public static class StarWarsData
{
    public static List<Droid> Droids = new List<Droid>
              {
                new Droid { Id = "123", Name = "R2-D2" , AppearsIn = [Episodes.EMPIRE, Episodes.NEWHOPE] },
                new Droid { Id = "456", Name = "R0-D5", AppearsIn = [Episodes.EMPIRE, Episodes.JEDI, Episodes.NEWHOPE] }
              };
}
~~~



### Query

~~~c#
public class Query
{
    public List<Droid> _droids = StarWarsData.Droids;


    [GraphQLMetadata("droid")]
    public Droid GetDroid(string id)
    {
        return _droids.FirstOrDefault(x => x.Id == id);
    }
}
~~~

- **`GetDroid` 方法**：定义了一个GraphQL字段 `droid`，用于根据ID获取一个 `Droid` 对象。



### 执行查询

~~~c#

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

~~~

- **模式定义**：
  - 定义了一个 `Droid` 类型，包含 `id` 和 `name` 字段， 且该字段必须存在值(**"!" 表示 Required**）。
  - 定义了一个 `Query` 类型，包含一个 `droid` 字段，该字段接受一个 `id` 参数并返回一个 `Droid` 类型； 且id参数必须提供值。

- **`queries`**：一个字符串数组，包含多个GraphQL查询。每个查询都调用了 `droid` 字段，并传递了一个 `id` 参数。

### 运行Schema First 示例

~~~cmd
dotnet run 7
~~~



运行结果

~~~bash
# Run _07QueryWithArguments_SF
Query: { droid(id: "123") {  id name   }}
---------
{
  "data": {
    "droid": {
      "id": "123",
      "name": "R2-D2"
    }
  }
}
Query: { droid(id: "456") {  id name   }}
---------
{
  "data": {
    "droid": {
      "id": "456",
      "name": "R0-D5"
    }
  }
}
Query: { droid(id: "789") {  id name   }}
---------
{
  "data": {
    "droid": null
  }
}
~~~

可以看到， 查询“123”， “456”， 都能返回结果； 但是"789"对应的数据不存在， 返回null。

## 2. 实现Query参数， Code First示例



Code First 示例： Query

~~~c#
public class StarWarsQuery : ObjectGraphType
{
 public List<Droid> _droids = StarWarsData.Droids;
 public StarWarsQuery()
 {
     Name = "Query";

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
~~~

- **`Field<DroidType>`**：定义了一个GraphQL字段 `droid`，返回类型是 `DroidType`，即一个 `Droid` 对象。
- **`Argument<IdGraphType>("id")`**：定义了一个参数 `id`，类型为 `IdGraphType`，表示客户端可以传递一个 `id` 参数来查询特定的 `Droid`。
- **`Resolve` 方法**：定义了字段的解析逻辑。
  - **`context.GetArgument<string?>("id")`**：从上下文中获取 `id` 参数的值。
  - 如果 `id` 不为 `null`，则从 `_droids` 列表中查找第一个 `Id` 等于 `id` 的 `Droid` 对象。
  - 如果 `id` 为 `null`，则返回 `_droids` 列表中的所有 `Droid` 对象。

### 执行查询

~~~c#
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
~~~



### 运行Code First 示例

~~~cmd
dotnet run 8
~~~

运行结果

~~~bash
# Run _08QueryWithArguments_CF
Query: {  droid(id: "123") {  id name   }}
---------
{
  "data": {
    "droid": {
      "id": "123",
      "name": "R2-D2"
    }
  }
}
Query: {  droid(id: "456") {  id name   }}
---------
{
  "data": {
    "droid": {
      "id": "456",
      "name": "R0-D5"
    }
  }
}
Query: {  droid(id: "789") {  id name   }}
---------
{
  "data": {
    "droid": null
  }
}
~~~



## 3. 实现变量传递

查询query和变量variables，可以分别提供给Execute方法:

### 执行查询

~~~c#
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
~~~

- **`query`**：GraphQL查询字符串，定义了一个名为 `DroidQuery` 的查询，它接受一个变量 `$droidId`，并使用该变量调用 `droid` 字段。
- **`variables`**：一个JSON字符串，表示查询的变量。这里定义了一个变量 `droidId`，其值为 `"123"`。需要通过反序列化方法转换为`Inputs`类型

- **`Inputs`**：GraphQL.NET 中的一个类型，用于表示查询的变量。



### 运行变量 示例

~~~cmd
dotnet run 9
~~~

运行结果

~~~bash
# Run _09QueryWithVariables
Query: 
{
 "query": "query DroidQuery($droidId: ID!) { droid(id: $droidId) { id name } }",
 "variables": {
   "droidId": "123"
 }
}

---------
{
  "data": {
    "droid": {
      "id": "123",
      "name": "R2-D2"
    }
  }
}
~~~

