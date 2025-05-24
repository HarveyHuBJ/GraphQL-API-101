namespace DataPlatform.GraphQL.API;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using GreenDonut;

public class CustomerDataLoader : BatchDataLoader<int, Customer>
{
    private readonly DataAccess dataAccess;

    public CustomerDataLoader(DataAccess dataAccess, IBatchScheduler batchScheduler, DataLoaderOptions options) : base(batchScheduler, options)
    {
        this.dataAccess = dataAccess;
    }

    protected override async Task<IReadOnlyDictionary<int, Customer>> LoadBatchAsync(IReadOnlyList<int> keys, CancellationToken cancellationToken)
    {
        return dataAccess.GetCustomerByIds(keys.ToArray());
    }
}

public class OrderDetailsDataLoader : BatchDataLoader<int, List<SalesOrderDetail>>
{
    private readonly DataAccess dataAccess;

    public OrderDetailsDataLoader(DataAccess dataAccess, IBatchScheduler batchScheduler, DataLoaderOptions options) : base(batchScheduler, options)
    {
        this.dataAccess = dataAccess;
    }

    protected override async Task<IReadOnlyDictionary<int, List<SalesOrderDetail>>> LoadBatchAsync(IReadOnlyList<int> keys, CancellationToken cancellationToken)
    {
        return dataAccess.GetOrderDetails(keys.ToArray());
    }
}

