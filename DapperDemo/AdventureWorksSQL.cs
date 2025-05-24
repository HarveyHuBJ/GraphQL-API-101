using Dapper;

using Microsoft.Data.SqlClient;

using System.Data.Common;
using System.Data.Odbc;
namespace DapperDemo;

public class AdventureWorksSQL : IDisposable, IAsyncDisposable
{
    private readonly string connectionString = "";

    public DbConnection DbConnection { get; }



    private AdventureWorksSQL(DbConnection connection)
    {
        this.DbConnection = connection;
    }
    public static AdventureWorksSQL Create(DbConnection connection)
    {
        return new AdventureWorksSQL(connection);
    }

    public static AdventureWorksSQL CreateSqlDB(string connectionString)
    {
        var dbConnection = new SqlConnection(connectionString);
        return new AdventureWorksSQL(dbConnection);
    }



    // Create by a ODBC DSN
    public static AdventureWorksSQL CreateViaDsn(string dsn, string user, string password)
    {
        var connection = new OdbcConnection($"DSN={dsn};UID={user};PWD={password}");
        connection.Open();
        return new AdventureWorksSQL(connection);
    }

    public IEnumerable<Product> GetProducts()
    {
        return DbConnection.Query<Product>("SELECT TOP 10 ProductID, Name, ProductNumber FROM SalesLT.Product");
    }

    public IEnumerable<Customer> GetCustomers()
    {
        return DbConnection.Query<Customer>("SELECT TOP 10 CustomerID, FirstName, LastName FROM SalesLT.Customer");
    }

    public IEnumerable<SalesOrderHeader> GetSalesOrderHeaders()
    {

        return DbConnection.Query<SalesOrderHeader>("SELECT TOP 10 SalesOrderID, OrderDate, CustomerID FROM SalesLT.SalesOrderHeader");

    }

    public IEnumerable<SalesOrderDetail> GetSalesOrderDetails()
    {

        return DbConnection.Query<SalesOrderDetail>("SELECT TOP 10 SalesOrderID, SalesOrderDetailID, ProductID, OrderQty FROM SalesLT.SalesOrderDetail");

    }

    public IEnumerable<dynamic> GetJoinedQuery(string query)
    {
        //string query = @"SELECT soh.SalesOrderID, soh.OrderDate, soh.CustomerID, 
        //                            sod.SalesOrderDetailID, sod.ProductID, sod.OrderQty,
        //                           c.FirstName, c.LastName
        //                    FROM SalesLT.SalesOrderHeader AS soh
        //                    JOIN SalesLT.Customer AS c ON soh.CustomerID = c.CustomerID
        //                    JOIN SalesLT.SalesOrderDetail AS sod ON soh.SalesOrderID = sod.SalesOrderID
        //                    ";




        return DbConnection.Query(query);


    }

    public void Dispose()
    {
        if (DbConnection == null)
            return;

        if (DbConnection != null && DbConnection.State != System.Data.ConnectionState.Closed)
        {
            DbConnection.Close();
        }

        ((IDisposable)DbConnection).Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return ((IAsyncDisposable)DbConnection).DisposeAsync();
    }
}