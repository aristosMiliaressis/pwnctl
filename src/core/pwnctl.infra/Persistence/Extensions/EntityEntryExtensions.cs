using Microsoft.EntityFrameworkCore.ChangeTracking;
using pwnctl.domain.BaseClasses;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.Extensions
{
    public static class EntityEntryExtensions
    {
        public static async Task LoadReferencesRecursivelyAsync(this EntityEntry entry, CancellationToken token = default, List<Type> refChain = null)
        {
            if (entry == null)
                return;

            // if reference chain is null(i.e recursive entry point) initialize the chain
            // else create a copy, to mentain a separet chain between every reqursive branch
            refChain = refChain == null
                    ? new List<Type>()
                    : new List<Type>(refChain);

            // if type exists in chain return to prevent infinit loop
            var type = entry.Entity.GetType();
            if (refChain.Contains(type))
                return;
            refChain.Add(type);

            if (!entry.IsKeySet)
            {
                var id = await new HashIdValueGenerator().NextAsync(entry);
                var tmpEntry = entry.Context
                            .ChangeTracker.Entries()
                            .FirstOrDefault(e => e.Metadata.ClrType == entry.Entity.GetType()
                                                && e.OriginalValues["Id"].Equals(id));
                if (tmpEntry == null)
                {
                    entry.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                }
                else entry = tmpEntry;
            }

            foreach (var reference in entry.References)
            {
                await reference.LoadAsync(token);
                await reference.TargetEntry.LoadReferencesRecursivelyAsync(token, refChain);
            }

            foreach (var collection in entry.Collections)
            {
                await collection.LoadAsync(token);

                var enumerator = collection.CurrentValue.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    await collection.FindEntry(enumerator.Current).LoadReferencesRecursivelyAsync(token, refChain);
                }
            }
        }
    }
}
