using Microsoft.Azure.Cosmos;

namespace JKang.EventSourcing.Persistence.CosmosDB
{
    public interface ICosmosContainerProvider<T>
    {
        Container Container { get; }
    }
}
