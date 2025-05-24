
namespace DapperDemo;

public class DsnSettings
{
    public DsnSettings(string dsn, string uid, string pwd)
    {
        this.DSN = dsn;
        this.UID = uid;
        this.PWD = pwd;
    }

    public DsnSettings(string driverPath, string host, string httpPath, string uid, string pwd)
    {
        this.Driver = driverPath;
        this.Host = host;
        this.HttpPath = httpPath;
        this.UID = uid;
        this.PWD = pwd;
    }
    public string DSN { get; private set; }
    public string UID { get; private set; }
    public string PWD { get; private set; }
    public string Driver { get; private set; }
    public string Host { get; private set; }
    public string HttpPath { get; private set; }
    public string Port { get; } = "443";
    public string SSL { get; } = "1";
    public string ThriftTransport { get;  } =   "2";
    public string AuthMech { get;  } = "3";
    public string Catalog { get; }
    public string Schema { get; }

    public override string ToString()
    {
        if (!string.IsNullOrEmpty(DSN))
        {
            return $"DSN={DSN};UID={UID};PWD={PWD}";
        }

        // ODBC driver connection string
        return $"DRIVER={Driver};HOST={Host};PORT={Port};HTTPPATH={HttpPath};SSL={SSL};THRIFTTRANSPORT={ThriftTransport};AuthMech={AuthMech};UID={UID};PWD={PWD}";
    }

    public static DsnSettings AdventureWorksSQLDsn = new DsnSettings("advworks", "CloudSA13a62506", "<password>");
    public static DsnSettings AdventureWorksADBDsn = new DsnSettings("azure databricks", "token", "<token>");
    public static DsnSettings AdventureWorksADBDsn2 = new DsnSettings(
                                                      //"c:\\Program Files\\Simba Spark ODBC Driver\\", //  driverPath
                                                        "{Simba Spark ODBC Driver}",
                                                        "adb-4150524051671565.5.azuredatabricks.net",  // host
                                                        "/sql/1.0/warehouses/c27216f66cb3905c",  // HTTP path
                                                         "token",  // UID
                                                         "<token>" // PWD
                                                         );
}