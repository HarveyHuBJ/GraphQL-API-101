namespace DataPlatform.GraphQL.API;
public class ProductType : ObjectType<Product>
{
    protected override void Configure(IObjectTypeDescriptor<Product> descriptor)
    {
        descriptor.Field(p => p.ProductID).Type<NonNullType<IdType>>();
        descriptor.Field(p => p.Name).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.ListPrice).Type<NonNullType<DecimalType>>();
        descriptor.Field(p => p.ProductNumber).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Color).Type<NonNullType<StringType>>();

    }
}

public class CustomerType : ObjectType<Customer>
{
    protected override void Configure(IObjectTypeDescriptor<Customer> descriptor)
    {
        descriptor.Field(p => p.CustomerID).Type<NonNullType<IdType>>();
        descriptor.Field(p => p.FirstName).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.LastName).Type<NonNullType<StringType>>();

    }

}

public class SalesOrderDetailType : ObjectType<SalesOrderDetail>
{
    protected override void Configure(IObjectTypeDescriptor<SalesOrderDetail> descriptor)
    {
        descriptor.Field(p => p.SalesOrderID).Type<NonNullType<IdType>>();
        descriptor.Field(p => p.SalesOrderDetailID).Type<NonNullType<IdType>>();
        descriptor.Field(p => p.ProductID).Type<NonNullType<IdType>>();

        // product
        //descriptor.Field(p => p.Product).Type<ProductType>();
    }
}

public class SalesOrderHeaderType : ObjectType<SalesOrderHeader>
{
    protected override void Configure(IObjectTypeDescriptor<SalesOrderHeader> descriptor)
    {
        descriptor.Field(p => p.SalesOrderID).Type<NonNullType<IdType>>();
        descriptor.Field(p => p.OrderDate).Type<NonNullType<DateTimeType>>();
        descriptor.Field(p => p.CustomerID).Type<NonNullType<IdType>>();

        //user data loader
        descriptor.Field("customer")
                    //.UseDataLoader<CustomerDataLoader>()
                    .Resolve<Customer?>(async _ =>
                    {
                        var loader = _.DataLoader<CustomerDataLoader>();
                        int customerID = _.Parent<SalesOrderHeader>().CustomerID;
                        var result = await loader.LoadAsync(customerID);
                        return result;
                    })
        ;

        descriptor.Field("details")
                  //.AddPagingArguments()
                  .UsePaging()
                  .UseFiltering()
                  .UseSorting()
                  .Resolve<List<SalesOrderDetail>>(async _ =>
                  {
                     
                      var loader = _.DataLoader<OrderDetailsDataLoader>();
                      int salesOrderID = _.Parent<SalesOrderHeader>().SalesOrderID;
                      var result = await loader.LoadAsync(salesOrderID);
                      return result;
                  }
            );
    }
}
