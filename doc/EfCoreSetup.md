# Setup EF Core event store

1. Install NuGet package [JKang.EventSourcing.Persistence.EfCore](https://www.nuget.org/packages/JKang.EventSourcing.Persistence.EfCore/)

    ```powershell
    PM> Install-Package JKang.EventSourcing.Persistence.EfCore
    ```

1. According to your requirement, select database engine and install corresponding NuGet package. For example [Microsoft.EntityFrameworkCore.InMemory](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.InMemory)

    ```powershell
    PM> Install-Package Microsoft.EntityFrameworkCore.InMemory
    ```

1. Create the DbContext containing the table for storing events

    ```csharp
        public class SampleDbContext : DbContext,
            IEventDbContext<GiftCard, Guid>
        {
            public SampleDbContext(DbContextOptions<SampleDbContext> options)
                : base(options)
            { }

            public DbSet<EventEntity<Guid>> GiftCardEvents { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.ApplyConfiguration(new EventEntityConfiguration<Guid>());
            }

            DbSet<EventEntity<Guid>> IEventDbContext<GiftCard, Guid>.GetEventDbSet() => GiftCardEvents;
        }
    ```

1. Register event sourcing services in ConfigureServices()

    ```csharp
    services
        .AddDbContext<SampleDbContext>(x => x.UseInMemoryDatabase("local"));

    services
        .AddEventSourcing(builder =>
        {
            builder
                .UseEfCoreEventStore<SampleDbContext, GiftCard, Guid>();
        });
    ```
