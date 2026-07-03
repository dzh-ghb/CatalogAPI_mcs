using Api.Model;
using Marten;
using Marten.Schema;

namespace Api.Data.Seed;

// класс для наполнения БД
public class InitializeBookDatabase : IInitialData
{
    // взаимодействие с БД происходит через IDocumentStore
    public async Task Populate(IDocumentStore store, CancellationToken cancellation)
    {
        using var session = store.LightweightSession();

        if (!await session.Query<Book>().AnyAsync())
        {
            session.Store<Book>(InitialData.Books);

            await session.SaveChangesAsync(cancellation);
        }
    }
}