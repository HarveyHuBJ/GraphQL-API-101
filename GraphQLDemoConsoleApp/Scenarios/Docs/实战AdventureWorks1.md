# 实战Adventure Works 1

本文介绍了如何在 **GraphQL.NET** 中使用 **Code First** 方法来定义一个GraphQL模式，并通过**Dapper**和**ODBC**连接到数据库来获取数据。



通过本例可以了解如何利用Dapper和ODBC实现GraphQL的查询。



以下是对代码的详细解释和思路：

### 1. **DbConnectionFactory 类**

```csharp
public class DbConnectionFactory
{
    public static DbConnection GetConnection()
    {
        var connectionString = @"DSN=advworks;UID=CloudSA13a62506;PWD=<pwd>;";
        var connection = new OdbcConnection(connectionString);
        return connection;
    }
}
```

- **`DbConnectionFactory`**：一个工厂类，用于创建和返回数据库连接。
- **`GetConnection` 方法**：创建并返回一个 `OdbcConnection` 对象，使用硬编码的连接字符串连接到数据库。

> 虽然本文使用的是SQL Server的Adventure Works 示例数据库， 但是考虑到Databricks 并不支持ADO.net的连接方式， 只支持ODBC的连接，因此本文数据库的连接也采用ODBC连接

### 2. **AdventureWorksSchema 类**

```csharp
public class AdventureWorksSchema : Schema
{
    public AdventureWorksSchema()
    {
        var repository = new AdventureWorksRepository();
        Query = new AdventureWorksQuery(repository);
    }
}
```

- **`AdventureWorksSchema`**：定义了GraphQL模式的根查询类型。
- **`Query` 属性**：设置为 `AdventureWorksQuery`，这是一个自定义的查询类型，定义了GraphQL查询的字段和解析逻辑。

### 3. **AdventureWorksRepository 类**

 **`AdventureWorksRepository`**：一个数据访问类，用于从数据库中获取数据。

~~~c#
public class AdventureWorksRepository
{
    private static readonly string schema = @"SalesLT";
    private static readonly string productTable = "Product";
    private static readonly string customerTable = "Customer";
    private static readonly string orderHeaderTable = "SalesOrderHeader";
    private static readonly string orderDetailTable = "SalesOrderDetail";
    private static readonly string productCategoryTable = "ProductCategory";

    public List<Product> GetAllProducts(int pageNumber, int pageSize)
    {
        using (DbConnection connection = DbConnectionFactory.GetConnection())
        {
            connection.Open();
            int skip = (pageNumber - 1) * pageSize;
            int take = pageSize;

            var sql = $@"
                SELECT *
                FROM {schema}.{productTable}
                ORDER BY ProductID
                OFFSET @skip ROWS
                FETCH NEXT @take ROWS ONLY";
            return connection.Query<Product>(sql, new { skip, take }).ToList();
        }
    }

    public Product GetProductById(int id)
    {
        using (DbConnection connection = DbConnectionFactory.GetConnection())
        {
            connection.Open();
            var sql = $@"
                SELECT *
                FROM {schema}.{productTable}
                WHERE ProductID = @id";
            return connection.QueryFirstOrDefault<Product>(sql, new { id });
        }
    }

    public List<ProductCategory> GetAllProductCategories()
    {
        using (DbConnection connection = DbConnectionFactory.GetConnection())
        {
            connection.Open();
            return connection.Query<ProductCategory>($"SELECT * FROM {schema}.{productCategoryTable}").ToList();
        }
    }

    public List<Customer> GetAllCustomers(int pageNumber, int pageSize)
    {
        using (DbConnection connection = DbConnectionFactory.GetConnection())
        {
            connection.Open();
            int skip = (pageNumber - 1) * pageSize;
            int take = pageSize;

            var sql = $@"
                SELECT *
                FROM {schema}.{customerTable}
                ORDER BY CustomerID
                OFFSET @skip ROWS
                FETCH NEXT @take ROWS ONLY";
            return connection.Query<Customer>(sql, new { skip, take }).ToList();
        }
    }

    public Customer GetCustomerById(int id)
    {
        using (DbConnection connection = DbConnectionFactory.GetConnection())
        {
            connection.Open();
            var sql = $@"
                SELECT *
                FROM {schema}.{customerTable}
                WHERE CustomerID = @id";
            return connection.QueryFirstOrDefault<Customer>(sql, new { id });
        }
    }

    public List<SalesOrderHeader> GetSalesOrderHeaders(int skip, int take)
    {
        using (DbConnection connection = DbConnectionFactory.GetConnection())
        {
            connection.Open();
            var sql = $@"
                SELECT *
                FROM {schema}.{orderHeaderTable}
                ORDER BY SalesOrderID
                OFFSET @skip ROWS
                FETCH NEXT @take ROWS ONLY";
            return connection.Query<SalesOrderHeader>(sql, new { skip, take }).ToList();
        }
    }

    public List<SalesOrderDetail> GetSalesOrderDetails(int salesOrderId)
    {
        using (DbConnection connection = DbConnectionFactory.GetConnection())
        {
            connection.Open();
            var sql = $@"
                SELECT *
                FROM {schema}.{orderDetailTable}
                WHERE SalesOrderID = @salesOrderId";
            return connection.Query<SalesOrderDetail>(sql, new { salesOrderId }).ToList();
        }
    }
}
~~~



- **方法**：
  - **`GetAllProducts`**：获取分页的产品列表。
  - **`GetProductById`**：根据ID获取单个产品。
  - **`GetAllProductCategories`**：获取所有产品类别。
  - **`GetAllCustomers`**：获取分页的客户列表。
  - **`GetCustomerById`**：根据ID获取单个客户。
  - **`GetSalesOrderHeaders`**：获取分页的销售订单头。
  - **`GetSalesOrderDetails`**：根据销售订单ID获取销售订单明细。

### 4. **AdventureWorksQuery 类**

csharp

复制

```csharp
public class AdventureWorksQuery : ObjectGraphType
{
    public AdventureWorksQuery(AdventureWorksRepository repository)
    {
        Name = "Query";

        Field<ListGraphType<ProductType>>("products")
            .Argument<IntGraphType>("pageIndex")
            .Argument<IntGraphType>("pageSize")
            .Resolve(context =>
            {
                var pageIndex = context.GetArgument<int>("pageIndex");
                var pageSize = context.GetArgument<int>("pageSize");
                return repository.GetAllProducts(pageIndex, pageSize);
            });

        Field<ProductType>("product")
            .Argument<IdGraphType>("id")
            .Resolve(context =>
            {
                int? id = context.GetArgument<int?>("id");
                if (id == null)
                {
                    return repository.GetAllProducts(1, 10);
                }
                return repository.GetProductById(id.Value);
            });
    }
}
```

- **`AdventureWorksQuery`**：定义了GraphQL查询的字段和解析逻辑。
- **字段**：
  - **`products`**：获取分页的产品列表。
  - **`product`**：根据ID获取单个产品。

### 执行代码

```csharp
public static async Task _21QueryProductById()
{
    var schema = new AdventureWorksSchema();
    var query = "query { product(id: \"680\" ) { productID  name  listPrice }}";
    var json = await schema.ExecuteAsync(_ => { _.Query = query; });

    Console.WriteLine($"Query: {query}");
    Console.WriteLine("---------");
    Console.WriteLine(json);
}
```



运行Scenario 21

~~~cmd
dotnet run 21
~~~



运行结果

~~~bash
# Run _21QueryProductById
Query: query { product(id: "680" ) { productID  name  listPrice }}
---------
{
  "data": {
    "product": {
      "productID": 680,
      "name": "HL Road Frame - Black, 58",
      "listPrice": 1431.5000
    }
  }
}
~~~



运行Scenario 22



~~~cmd
dotnet run 22
~~~



运行结果

~~~bash
# Run _22QueryProducts
Query: query { products(pageIndex: 1, pageSize:3 ) { productID  name  listPrice }}
---------
{
  "data": {
    "products": [
      {
        "productID": 680,
        "name": "HL Road Frame - Black, 58",
        "listPrice": 1431.5000
      },
      {
        "productID": 706,
        "name": "HL Road Frame - Red, 58",
        "listPrice": 1431.5000
      },
      {
        "productID": 707,
        "name": "Sport-100 Helmet, Red",
        "listPrice": 34.9900
      }
    ]
  }
}
~~~

~~~bash
# Run _22QueryProducts
Query: query { products(pageIndex: 2, pageSize:5 )  { productID  name  listPrice }}
---------
{
  "data": {
    "products": [
      {
        "productID": 710,
        "name": "Mountain Bike Socks, L",
        "listPrice": 9.5000
      },
      {
        "productID": 711,
        "name": "Sport-100 Helmet, Blue",
        "listPrice": 34.9900
      },
      {
        "productID": 712,
        "name": "AWC Logo Cap",
        "listPrice": 8.9900
      },
      {
        "productID": 713,
        "name": "Long-Sleeve Logo Jersey, S",
        "listPrice": 49.9900
      },
      {
        "productID": 714,
        "name": "Long-Sleeve Logo Jersey, M",
        "listPrice": 49.9900
      }
    ]
  }
}
~~~

