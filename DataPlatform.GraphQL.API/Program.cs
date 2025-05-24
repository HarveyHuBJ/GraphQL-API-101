using HotChocolate.AspNetCore.Voyager;
using HotChocolate.Types.Pagination;

namespace DataPlatform.GraphQL.API
{


    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;

            // Add services to the container.

            //services.AddTransient<IDependencyResolver>(x => new FuncDependencyResolver(x.GetRequiredService));



            builder.Services
                    .AddGraphQLServer() // ָ��ģʽ����
                    .AddFiltering()
                    .AddSorting()
                    .ModifyPagingOptions(c => { c.DefaultPageSize = 2; c.IncludeTotalCount = true; }) // ����Ĭ�Ϸ�ҳ
                    //.UseDocumentCache()
                    //.UseOperationCache()
                    .AddQueryType<Query>()
                    //.AddDataLoader()
                    .AddTypes(typeof(ProductType)
                            , typeof(CustomerType)
                            , typeof(SalesOrderDetailType)
                            , typeof(SalesOrderHeaderType)
                            , typeof(QueryArgs)
                            //, typeof(FilterGroup)
                            )

                    //.AddInMemoryQueryStorage()

                    //.UseDocumentCache()
                    //.AddGraphTypes(typeof(ProductType).Assembly)
                    ;
            builder.Services.AddSingleton<DataAccess>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseGraphQL().UseVoyager();
            app.MapGraphQL("/graphql", schemaName: null);
            app.UseVoyager("/graphql", "/graphql-v");
            app.Run();
        }
    }
}
