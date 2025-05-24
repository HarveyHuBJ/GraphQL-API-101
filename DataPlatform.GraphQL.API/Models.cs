namespace DataPlatform.GraphQL.API;
 
[GraphQLDescription("To represent a product with name, color, and other properties")]
public class Product
{
    [GraphQLDescription("The unique identifier for the product")]
    [ID]
    public int ProductID { get; set; }

    [GraphQLDescription("The name of the product")]
    public required string Name { get; set; }

    [GraphQLDescription("The product number, like 'S700'")]
    public string? ProductNumber { get; set; }

    [GraphQLDescription("The list price of the product")]
    public Decimal? ListPrice { get; set; }

    [GraphQLDescription("The color of the product")]
    public string? Color { get; set; }
    // Add other properties as needed
}

public class Customer
{
    public int CustomerID { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    // Add other properties as needed
}

public class SalesOrderHeader
{
    public int SalesOrderID { get; set; }
    public DateTime OrderDate { get; set; }
    public int CustomerID { get; set; }
    // Add other properties as needed
}

public class SalesOrderDetail
{
    public int SalesOrderID { get; set; }
    public int SalesOrderDetailID { get; set; }
    public int ProductID { get; set; }
    public int OrderQty { get; set; }
    // Add other properties as needed
}
