using Dapper;

using System.Text.Json.Serialization;

namespace DapperDemo
{
    internal class Program
    {
        private static string user = "CloudSA13a62506";
        private static string password = "<password>";
        private static string connectionString =
            "Server=tcp:sqlsvr-mdd-001.database.windows.net,1433;Initial Catalog=sqldb-advwks;Persist Security Info=False;User ID={0};Password={1};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        private static string dsn = "advworks";

        private static string ConnectionString => string.Format(connectionString, user, password);
        static void Main(string[] args)
        {
            //SqlMapper.Settings(Console.WriteLine);
            //using (var adventureWorks =   AdventureWorks.Create(connectionString))
            IAdventureWorks adventureWorks =
                       AdventureWorksADB.CreateViaDsn(DsnSettings.AdventureWorksSQLDsn);

            try
            {

            //BasicQueryEntities(adventureWorks);
            //JoinQuery(adventureWorks); 
            ParameterQuery(adventureWorks);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
           
        }

        /// <summary>
        /// Basic query entities, using Dapper,
        /// Retrieve products, customers, sales order headers, and sales order details
        /// </summary>
        /// <param name="adventureWorks"></param>
        private static void BasicQueryEntities(AdventureWorksSQL adventureWorks)
        {
            Console.WriteLine("Products:");
            foreach (var product in adventureWorks.GetProducts())
            {
                Console.WriteLine($"Product ID: {product.ProductID}, Name: {product.Name}");
            }

            Console.WriteLine("\nCustomers:");
            foreach (var customer in adventureWorks.GetCustomers())
            {
                Console.WriteLine($"Customer ID: {customer.CustomerID}, Name: {customer.FirstName} {customer.LastName}");
            }

            Console.WriteLine("\nSales Order Headers:");
            foreach (var orderHeader in adventureWorks.GetSalesOrderHeaders())
            {
                Console.WriteLine($"Order ID: {orderHeader.SalesOrderID}, Date: {orderHeader.OrderDate}");
            }

            Console.WriteLine("\nSales Order Details:");
            foreach (var orderDetail in adventureWorks.GetSalesOrderDetails())
            {
                Console.WriteLine($"Order ID: {orderDetail.SalesOrderID}, Product ID: {orderDetail.ProductID}, Quantity: {orderDetail.OrderQty}");
            }
        }

        private static void ParameterQuery(IAdventureWorks adventureWorks) { 
        
            Console.WriteLine("\nSales product:");

            int skip = 2;
            int take = 5;
            var sql = $@"
                SELECT  *
                FROM saleslt.product
                ORDER BY ProductID  -- It's important to have an ORDER BY for predictable pagination
                OFFSET {skip} ROWS
                FETCH NEXT {take} ROWS ONLY";
      
            var products = adventureWorks.Query<Product>(sql, new { skip, take }).ToList();

            foreach (var item in products)
            {
                Console.WriteLine($"{item.ProductID}-{item.ProductNumber}" );
            }
        }

        /// <summary>
        /// Get sales order headers and display their details.
        /// </summary>
        /// <param name="adventureWorks"></param>
        private static void JoinQuery(IAdventureWorks adventureWorks)
        {
            Console.WriteLine("\nSales Order Headers:");

            string query = @"SELECT soh.sales_order_id, soh.order_date, soh.customer_id, 
                                        sod.sales_order_detail_id, sod.product_id, sod.order_qty,
                                       c.first_name, c.last_name
                                FROM ods__adwk__sales_lt.ods__adwk__sales_lt__sales_order_header AS soh
                                JOIN ods__adwk__sales_lt.ods__adwk__sales_lt__customer AS c ON soh.customer_id = c.customer_id
                                JOIN ods__adwk__sales_lt.ods__adwk__sales_lt__sales_order_detail AS sod ON soh.sales_order_id = sod.sales_order_id
                                ";
            foreach (var item in adventureWorks.Query<dynamic>(query))
            {
                // soh.SalesOrderID, soh.OrderDate, soh.CustomerID, 
                //sod.SalesOrderDetailID, sod.ProductID, sod.OrderQty
                //                   c.FirstName, c.LastName
                Console.WriteLine($"SalesOrderID: {item.sales_order_id}," +
                    $" OrderDate: {item.order_date} " +
                    $" SalesOrderDetailID: {item.SalesOrderDetailID} " +
                    $" ProductID: {item.product_id} " +
                    $" FirstName: {item.first_name} " +
                    $" LastName: {item.last_name} " +
                    $" OrderQty: {item.order_qty} " +
                    $" CustomerID: {item.customer_id}"

                    );
            }

        }
    }


}
