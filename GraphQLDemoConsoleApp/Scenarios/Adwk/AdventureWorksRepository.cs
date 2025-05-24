using GraphQLDemoConsoleApp.Scenarios.Adwk.Models;
using Dapper;


using System.Data.Common;
using System.Data.Odbc;
using System;

namespace GraphQLDemoConsoleApp.Scenarios.Adwk
{
    public class AdventureWorksRepository
    {
         

        #region AdventureWorks Sql Server Database
        private static readonly string schema = @"SalesLT";

        private static readonly string productTable = "Product";
        private static readonly string customerTable = "Customer";
        private static readonly string orderHeaderTable = "SalesOrderHeader";
        private static readonly string orderDetailTable = "SalesOrderDetail";
        private static readonly string productCategoryTable = "ProductCategory";

        #endregion

        public AdventureWorksRepository( )
        {
            
        }



        public List<Product> GetAllProducts(int pageNumber, int pageSize)
        {

            if  (pageNumber < 1 || pageSize < 1)
            {
                throw new ArgumentException("Invalid page number or page size");
            }

            using (DbConnection connection = DbConnectionFactory.GetConnection())
            {
                connection.Open();
                int skip = (pageNumber - 1) * pageSize;
                int take = pageSize;

                //考虑在SQL中增加翻页
                var sql = $@"
                SELECT *
                FROM {schema}.{productTable}
                ORDER BY ProductID  -- It's important to have an ORDER BY for predictable pagination
                OFFSET {skip} ROWS
                FETCH NEXT {take} ROWS ONLY";
                var products = connection.Query<Product>(sql ).ToList();

                // var sql = $@"
                // SELECT top 10 *
                // FROM {schema}.{productTable}
                // ORDER BY ProductID
//";

                //var products = connection.Query<Product>(sql ).ToList();

                 return products;
            }
        }


        public Product GetProductById(int id)
        {
            using (DbConnection connection = DbConnectionFactory.GetConnection())
            {
                connection.Open();
                var result = connection.QueryFirstOrDefault<Product>($"SELECT * FROM {schema}.{productTable} WHERE ProductID = {id}");
                return result;
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
                // 考虑分页
                int skip = (pageNumber - 1) * pageSize;
                int take = pageSize;

                var sql = @$"SELECT * FROM {schema}.{customerTable} 
                ORDER BY CustomerID  -- It's important to have an ORDER BY for predictable pagination
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
                WHERE CustomerID = @id
            ";

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
}