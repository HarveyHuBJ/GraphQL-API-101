using GraphQL;
using GraphQL.Types;

using GraphQLDemoConsoleApp.Scenarios.Adwk.GraphTypes;
using GraphQLDemoConsoleApp.Scenarios.Adwk.Models;
using static GraphQLDemoConsoleApp.Scenarios.Adwk.GraphTypes.ProductType;

namespace GraphQLDemoConsoleApp.Scenarios.Adwk
{
    public class AdventureWorksQuery : ObjectGraphType
    {
        public AdventureWorksQuery(AdventureWorksRepository repository)
        {
            Name = "Query";

            // products with pagination
            Field<ListGraphType<ProductType>>("products")
               .Argument<IntGraphType>("pageIndex")
               .Argument<IntGraphType>("pageSize")
               .Resolve(context =>
               {
                   var pageIndex = context.GetArgument<int>("pageIndex");
                   var pageSize = context.GetArgument<int>("pageSize");

                   return repository.GetAllProducts(pageIndex, pageSize);
               }
            );

            // product by id
            Field<ProductType>("product")
              .Argument<IdGraphType>("id")
              .Resolve(context =>
              {
                  int? id = context.GetArgument<int?>("id");
                  if (id == null)
                  {
                      return repository.GetAllProducts(1, 10);
                  }
                  return repository.GetProductById(id.Value);
              }
            );

            // Field<ListGraphType<ProductType>>("products")
            //     .Resolve(_ => repository.GetAllProducts(1, 3));

            //            // customers with pagination
            //            Field<ListGraphType<CustomerType>>("customers")
            //                .Argument<IntGraphType>("pageIndex")
            //                .Argument<IntGraphType>("pageSize")
            //                .Resolve(context =>
            //                {
            //                    var pageIndex = context.GetArgument<int>("pageIndex");
            //                    var pageSize = context.GetArgument<int>("pageSize");
            //                    return repository.GetAllCustomers(pageIndex, pageSize);
            //                }
            //            );

            //            // customer by id
            //            Field<CustomerType>("customer")
            //                .Argument<IntGraphType>("id")
            //                .Resolve(context =>
            //                {
            //                    var id = context.GetArgument<int>("id");
            //                    return repository.GetCustomerById(id);
            //                }
            //            );

            //            // product categories
            //            Field<ListGraphType<ProductCategoryType>>("productCategories")
            //                .Resolve(context =>
            //                repository.GetAllProductCategories()
            //);

            //            // sales order headers
            //            Field<ListGraphType<SalesOrderHeaderType>>("orders")
            //                .Argument<IntGraphType>("pageIndex")
            //                .Argument<IntGraphType>("pageSize")
            //                .Resolve(context =>
            //                {
            //                    var pageIndex = context.GetArgument<int>("pageIndex");
            //                    var pageSize = context.GetArgument<int>("pageSize");
            //                    return repository.GetSalesOrderHeaders(pageIndex, pageSize);
            //                }
            //                );

            //            // sales order header by id
            //            Field<SalesOrderHeaderType>("order")
            //                .Argument<IntGraphType>("id")
            //                .Resolve(context =>
            //                repository.GetSalesOrderHeaderById(context.GetArgument<int>("id"))
            //);
        }
    }
}