
namespace DapperDemo;
public class Product
{
    public int ProductID { get; set; }
    public required string Name { get; set; }
    public string? ProductNumber { get; set; }
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
