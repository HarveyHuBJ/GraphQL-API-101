# S3 Query(1)

## 1 Query的格式

GraphQL 查询是一种用于请求数据的格式，它允许客户端精确地指定需要的数据。以下是 GraphQL 查询的基本格式和结构，以及一些示例。

### 1.1. **基本结构**

GraphQL 查询通常由以下几部分组成：

- **查询名称**（可选）：用于标识查询的名称。
- **字段**：客户端需要的数据字段。
- **参数**（可选）：传递给查询的参数。
- **变量**（可选）：传递给查询的变量。
- **嵌套字段**：可以嵌套查询关联对象的字段。
- **片段**（可选）：可以定义可重用的字段集合。

### 1.2. **基本查询格式**

```graphql
query QueryName($variable: Type) {
  field1(arg: value)
  field2
  field3 {
    nestedField1
    nestedField2
  }
}
```

其中`$variable`是变量， `arg` 是参数

如果只查询一个类型， 'query' 也可以省略。

### 1.3 示例

~~~c#
public static async Task _05QuerySimple()
{

    var schema = new Schema { Query = new StarWarsQuery() };

    var queries = new string[] {
"query MyHeroQuery { hero {  id name  }   }  ",
"query   { hero {  id name  }   }  ",
"{hero {  id name  } }    "
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

运行

~~~cmd
dotnet run 5
~~~

运行结果

~~~bash
# Run _05QuerySimple
Query: query MyHeroQuery { hero {  id name  }   }  
---------
{
  "data": {
    "hero": {
      "id": "1",
      "name": "R2-D2"
    }
  }
}
Query: query   { hero {  id name  }   }
---------
{
  "data": {
    "hero": {
      "id": "1",
      "name": "R2-D2"
    }
  }
}
Query: {hero {  id name  } }
---------
{
  "data": {
    "hero": {
      "id": "1",
      "name": "R2-D2"
    }
  }
}
~~~

可以看到3种查询语句， 返回的结果是一致的。 

## 2. **总结**

GraphQL 查询的格式非常灵活，允许客户端精确地请求所需的数据。通过定义查询名称、字段、参数和片段，客户端可以高效地获取所需的数据，同时减少不必要的数据传输。

