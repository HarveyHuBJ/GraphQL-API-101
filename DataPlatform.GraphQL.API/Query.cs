namespace DataPlatform.GraphQL.API;

public class Query
{
    private readonly DataAccess _dataAccess;

    public Query(DataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }


    [GraphQLDescription("Retrieve product by id")]
    public Product? GetProduct(int productId) => _dataAccess.GetProduct(productId);


    [GraphQLDescription("Retrieve sales order data by id")]
    public SalesOrderHeader? GetOrder(int orderId) => _dataAccess.GetSalesOrder(orderId);


    [GraphQLDescription("Retrieve customer data by id")]
    public Customer? GetCustomer(int customerId) => _dataAccess.GetCustomer(customerId);

    [UsePaging, UseSorting, UseFiltering]
    [GraphQLDescription("Retrieve product list, supports pagination, sorting, and filtering")]
    public IQueryable<Product> GetProducts() => _dataAccess.GetProducts(null).AsQueryable();
    [UsePaging, UseSorting, UseFiltering]
    [GraphQLDescription("Retrieve customer list, supports pagination, sorting, and filtering")]
    public IQueryable<Customer> GetCustomers() => _dataAccess.GetCustomers(null).AsQueryable();


    //[Authorize(Roles ="Sales")]
    [UsePaging, UseSorting, UseFiltering]
    [GraphQLDescription("Retrieve order list, supports pagination, sorting, and filtering")]
    public IQueryable<SalesOrderHeader> GetOrders() => _dataAccess.GetOrders(null).AsQueryable();

}

//public IQueryable<Customer> GetCustomerByIds(QueryArgs args) => _dataAccess.GetCustomerByIds(args).AsQueryable();
//public IQueryable<SalesOrderHeader> GetOrders(QueryArgs args) => _dataAccess.GetOrders(args).AsQueryable();
//public IQueryable<Product> GetProducts(QueryArgs args) => _dataAccess.GetProducts(args).AsQueryable();
