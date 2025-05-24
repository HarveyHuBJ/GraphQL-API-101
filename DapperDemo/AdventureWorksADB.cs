using Dapper;

using Microsoft.Data.SqlClient;

using System.Data.Common;
using System.Data.Odbc;
namespace DapperDemo;

public interface IAdventureWorks  
{
    IEnumerable<T> Query<T>(string query, object? parameters = null);
}
public class AdventureWorksADB :  IAdventureWorks
{
    private string connectionString;

    private AdventureWorksADB(string connection)
    {
        this.connectionString = connection;
    }

    // Create by a ODBC DSN
    public static AdventureWorksADB CreateViaDsn(DsnSettings dsnSettings)
    {
        return new AdventureWorksADB(dsnSettings.ToString());
    }
     

    public IEnumerable<T> Query<T>(string query, object? parameters)
    {
        using (var dbConnection = new OdbcConnection(connectionString))
        {
            dbConnection.Open();
            return dbConnection.Query<T>(query, parameters);
        }
    }
     
}