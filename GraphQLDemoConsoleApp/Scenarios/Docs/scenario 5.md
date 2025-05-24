# S5 Query(3) - 别名与片段

本文介绍如何在 **GraphQL.NET** 中使用 **片段（Fragments）** 来定义可重用的字段集合，并在查询中复用这些字段。

执行查询

~~~c#
public static async Task _11QueryWithFragments()
{
    // Alias : leftComparison rightComparison
    // Fragment : droid
    var query = @"
query {
leftComparison: droid(id: ""123"") {
...comparisonFields
}
rightComparison: droid(id: ""456"") {
...comparisonFields
}
}

fragment comparisonFields on Droid {
id name appearsIn
}
"
    ;

    var schema = new Schema { Query = new StarWarsQuery() };
    var json = await schema.ExecuteAsync(_ =>
    {
        _.Query = query;
    });


    Console.WriteLine($"Query: {query}");
    Console.WriteLine("---------");
    Console.WriteLine(json);
}
~~~

- **查询定义**：
  - 定义了一个查询，包含两个字段 `leftComparison` 和 `rightComparison`，它们分别调用了 `droid` 字段，并传递了不同的 `id` 参数。
  - 使用了片段 `...comparisonFields`，表示在这两个字段中复用 `comparisonFields` 片段定义的字段。
- **片段定义**：
  - 定义了一个片段 `comparisonFields`，它适用于 `Droid` 类型。
  - 片段中包含字段 `id`、`name` 和 `appearsIn`，这些字段将在使用该片段的地方被展开。

执行查询

~~~cmd
dotnet run 11
~~~



运行结果

~~~bash
# Run _11QueryWithFragments
Query: 
query {
  leftComparison: droid(id: "123") {
    ...comparisonFields
  }
  rightComparison: droid(id: "456") {
    ...comparisonFields
  }
}

fragment comparisonFields on Droid {
  id name appearsIn
}

---------
{
  "data": {
    "leftComparison": {
      "id": "123",
      "name": "R2-D2",
      "appearsIn": [
        "EMPIRE",
        "NEWHOPE"
      ]
    },
    "rightComparison": {
      "id": "456",
      "name": "R0-D5",
      "appearsIn": [
        "EMPIRE",
        "JEDI",
        "NEWHOPE"
      ]
    }
  }
}
~~~



通过定义片段，可以减少代码重复，提高查询的可维护性和可读性。