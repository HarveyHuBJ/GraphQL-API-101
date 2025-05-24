# S1 认识GraphQL

本文将以“Hello World" 为例， 介绍GraphQL.net的基本实现原理。

## 初识GRAPHQL

### Hello World

```c#
var schema = Schema.For(@"
  type Query {
    hello: String
  }
");
var serializer = new GraphQLSerializer();
var json = await schema.ExecuteAsync(serializer, _ =>
{
    _.Query = "{ hello }";
    _.Root = new { Hello = "Hello World!" };
});

Console.WriteLine(json);
```

要点：

- Schema
- Query
- Resolver
- ExecuteAsync() , 需要namespace : GraphQL.SystemTextJson

### Prerequistes

- install dotnet 8
- install nuget package : `dotnet add package GraphQL`
- install nuget package : `dotnet add package GraphQL.SystemTextJson`

```xml
    <PackageReference Include="GraphQL" Version="8.5.0" />
    <PackageReference Include="GraphQL.SystemTextJson" Version="8.5.0" />
```

### GraphQL 查询：ExecuteAsync

| ExecutionOptions 属性     | 类型                            | 说明                                                                                                               | 示例                                                     |
| ------------------------- | ------------------------------- | ------------------------------------------------------------------------------------------------------------------ | -------------------------------------------------------- |
| **Schema**          | `ISchema`                     | 定义了GraphQL服务的模式，包括可用的类型、字段、查询和变异等。这是GraphQL操作的基础，决定了客户端可以请求哪些数据。 | `new Schema { Query = new QueryType() }`               |
| Root                      | Object?                         | 第一层解析对象                                                                                                     |                                                          |
| **Variables**       | `IDictionary<string, object>` | 用于传递GraphQL查询中的变量，这些变量可以在查询中动态使用，避免硬编码。                                            | `new Dictionary<string, object> { { "userId", 123 } }` |
| **OperationName**   | `string`                      | 在GraphQL请求中指定要执行的操作名称，当请求中包含多个查询或变异时，用于区分具体执行哪一个。                        | `"getUserDetails"`                                     |
| **RequestServices** | `IServiceProvider`            | 提供对应用程序服务的访问，例如依赖注入容器中的服务。在解析GraphQL查询时，可以使用这些服务来执行业务逻辑。          | `services.BuildServiceProvider()`                      |

## 示例说明

```cmd
dotnet run 1
```

结果：

```bash
# Run _01HelloWorld
Query: { hello }
---------
{"data":{"hello":"Hello World!"}}
```

## 其他

返回的类型是JSON类型， 字段是由 `GraphQL.SystemTextJson` 自动将名称转换为**驼峰**大小写。
