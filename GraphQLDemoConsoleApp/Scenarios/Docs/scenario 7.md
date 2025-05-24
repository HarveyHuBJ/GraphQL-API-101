# S7 元数据

使用GraphQL 还可以获得元数据



执行查询

~~~c#
  public static async Task _13QueryMetadata()
  {
      var schema = new Schema { Query = new StarWarsQuery() };

      var query = "query   {     __schema { types { kind name fields { name description}}}    }  ";
      var json = await schema.ExecuteAsync(_ =>
      {
          _.Query = query;
      });


      Console.WriteLine($"Query: {query}");
      Console.WriteLine("---------");
      Console.WriteLine(json);

  }
~~~

查询元数据的query示例：

~~~json
// 示例1
query {
  __schema {
    types {
      kind
      name
      fields {
        name
        description
      }
    }
  }
}


~~~

~~~ json
// 示例2
query {
  __type(name: "__Directive") {
    name
    kind
    description
  }
}
~~~



运行

~~~cmd
dotnet run 13
~~~



查询结果

~~~bash
# Run _13QueryMetadata
Query: query   {     __schema { types { kind name fields { name description}}}    }
---------
{
  "data": {
    "__schema": {
      "types": [
        {
          "kind": "OBJECT",
          "name": "__Schema",
          "fields": [
            {
              "name": "description",
              "description": null
            },
            {
              "name": "types",
              "description": "A list of all types supported by this server."
            },
            {
            ...
~~~

