namespace DataPlatform.GraphQL.API;

using Dapper;

using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;

public class DataAccess
{
    private readonly string _connectionString;

    public DataAccess(IConfiguration configuration)
    {
        var temp = configuration.GetConnectionString("AdventureWorksConnection");

        var password = Environment.GetEnvironmentVariable("ADWK_DB_PASSWORD");
        _connectionString = temp.Replace("${ADWK_DB_PASSWORD}", password);

   
    }

    public List<Product> GetProducts(QueryArgs? args)
    {
        using (IDbConnection db = new OdbcConnection(_connectionString))
        {
            string query = "SELECT ProductID, Name, ListPrice FROM SalesLT.Product";
            if (args != null)
            {
                query = args.Decorate(query);
            }
            return db.Query<Product>(query).ToList();
        }
    }

    public Product? GetProduct(int productId)
    {
        using (IDbConnection db = new OdbcConnection(_connectionString))
        {
            string query = $"SELECT ProductID, Name, ListPrice, Color, ProductNumber FROM SalesLT.Product WHERE ProductID = {productId}";

            return db.QuerySingleOrDefault<Product>(query);
        }
    }

    // Customer , Order , OrderDetail 
    public IEnumerable<Customer> GetCustomers(QueryArgs args)
    {
        using (IDbConnection db = new OdbcConnection(_connectionString))
        {
            string query = "SELECT CustomerID, FirstName, LastName FROM SalesLT.Customer";
            if (args != null)
            {
                query = args.Decorate(query);
            }

            return db.Query<Customer>(query);
        }
    }

    public IEnumerable<SalesOrderHeader> GetOrders(QueryArgs? args )
    {
        using (IDbConnection db = new OdbcConnection(_connectionString))
        {
            string query = "SELECT SalesOrderID, OrderDate, CustomerID FROM SalesLT.SalesOrderHeader ";
            if (args != null)
            {
                query = args.Decorate(query);
            }

            return db.Query<SalesOrderHeader>(query);
        }
    }

    public IEnumerable<SalesOrderDetail> GetOrderDetails(int orderId, QueryArgs? args)
    {
        using (IDbConnection db = new OdbcConnection(_connectionString))
        {
            string query = $"SELECT SalesOrderID, SalesOrderDetailID, ProductID, OrderQty FROM SalesLT.SalesOrderDetail ";
            if (args != null)
            {
                query = args.Decorate(query);
            }
            return db.Query<SalesOrderDetail>(query);


        }
    }

    public Dictionary<int, List<SalesOrderDetail>> GetOrderDetails(int[] orderIds)
    {
        using (IDbConnection db = new OdbcConnection(_connectionString))
        {
            var id_list = string.Join(",", orderIds);
            string query = $"SELECT SalesOrderID, SalesOrderDetailID, ProductID, OrderQty FROM SalesLT.SalesOrderDetail WHERE SalesOrderID in ({id_list}) ";

            return db.Query<SalesOrderDetail>(query)
                   .GroupBy(p => p.SalesOrderID)
                   .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
    public SalesOrderHeader? GetSalesOrder(int orderId)
    {
        using (IDbConnection db = new OdbcConnection(_connectionString))
        {
            return db.QueryFirstOrDefault<SalesOrderHeader>($"SELECT SalesOrderID, OrderDate, CustomerID FROM SalesLT.SalesOrderHeader WHERE SalesOrderID = {orderId}");
        }
    }


    public Customer? GetCustomer(int customerId)
    {
        using (IDbConnection db = new OdbcConnection(_connectionString))
        {
            return db.QueryFirstOrDefault<Customer>($"SELECT * FROM SalesLT.Customer WHERE CustomerID = {customerId}");
        }
    }

    public Dictionary<int, Customer> GetCustomerByIds(int[] customerIds)
    {
        using (IDbConnection db = new OdbcConnection(_connectionString))
        {
            var id_list = string.Join(",", customerIds);
            var result = db.Query<Customer>($"SELECT * FROM SalesLT.Customer WHERE CustomerID in ({id_list})")
                    .ToDictionary(p => p.CustomerID);

            return result;
        }
    }
}

