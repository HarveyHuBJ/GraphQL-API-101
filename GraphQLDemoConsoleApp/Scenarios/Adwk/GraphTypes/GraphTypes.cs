using GraphQL.Federation;
using GraphQL.Types;

using GraphQLDemoConsoleApp.Scenarios.Adwk.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQLDemoConsoleApp.Scenarios.Adwk.GraphTypes
{
    public class ProductType : ObjectGraphType<Product>
    {
        public ProductType()
        {
            //this.Key("productID");
            Field(x => x.ProductID).Description("The ID of the product.");
            Field(x => x.Name).Description("The name of the product.");
            Field(x => x.ProductNumber).Description("The product number.");
            Field(x => x.ListPrice).Description("The list price of the product.");
        }

    }

    public class CustomerType : ObjectGraphType<Customer>
    {
        public CustomerType()
        {
            Field(x => x.CustomerID).Description("The ID of the customer.");
            Field(x => x.AccountNumber).Description("The account number of the customer.");
            Field(x => x.CompanyName).Description("The company name of the customer.");
        }
    }
    public class SalesOrderHeaderType : ObjectGraphType<SalesOrderHeader>
    {
        public SalesOrderHeaderType()
        {
            Field(x => x.SalesOrderID).Description("The ID of the sales order.");
            Field(x => x.CustomerID).Description("The ID of the customer.");
            Field(x => x.TotalDue).Description("The total due for the sales order.");
            Field(x => x.OrderDate).Description("The order date of the sales order.");
        }
    }
    public class SalesOrderDetailType : ObjectGraphType<SalesOrderDetail>
    {
        public SalesOrderDetailType()
        {
            Field(x => x.SalesOrderID).Description("The ID of the sales order.");
            Field(x => x.ProductID).Description("The ID of the product.");
            Field(x => x.OrderQty).Description("The order quantity.");
            Field(x => x.UnitPrice).Description("The unit price of the product.");
        }
    }

    public class ProductCategoryType : ObjectGraphType<ProductCategory>
    {
        public ProductCategoryType()
        {
            Field(x => x.ProductCategoryID).Description("The ID of the product category.");
            Field(x => x.Name).Description("The name of the product category.");
        }
    }
}
