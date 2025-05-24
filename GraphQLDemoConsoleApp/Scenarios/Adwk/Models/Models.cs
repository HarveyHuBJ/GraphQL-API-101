using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQLDemoConsoleApp.Scenarios.Adwk.Models
{
 
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string ProductNumber { get; set; }
        public decimal ListPrice { get; set; }
    }

    public class Customer
    {
        public int CustomerID { get; set; }
        public string AccountNumber { get; set; }
        public string CompanyName { get; set; }
    }
    public class SalesOrderHeader
    {
        public int SalesOrderID { get; set; }
        public int CustomerID { get; set; }
        public decimal TotalDue { get; set; }
        public System.DateTime OrderDate { get; set; }
    }
    public class SalesOrderDetail
    {
        public int SalesOrderID { get; set; }
        public int ProductID { get; set; }
        public int OrderQty { get; set; }
        public decimal UnitPrice { get; set; }
    }
    public class ProductCategory
    {
        public int ProductCategoryID { get; set; }
        public string Name { get; set; }
    }
}
