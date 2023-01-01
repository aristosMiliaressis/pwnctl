using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace pwnctl.infra.Persistence.Extensions
{
    public static class EntityEntryExtensions
    {
        public static async Task LoadReferencesRecursivelyAsync(this EntityEntry entry, Func<Type, bool> filter, CancellationToken token = default, List<Type> refChain = null)
        {
            if (entry == null)
                return;

            // if reference chain is null(i.e recursive entry point) initialize the chain
            if (refChain == null)
                refChain = new List<Type>();

            // create a copy of the reference chain, 
            // this is done to mentain a separet chain 
            // between every reqursive branch
            refChain = new List<Type>(refChain);

            // if type exists in chain return to prevent infinit loop
            var type = entry.Entity.GetType();
            if (refChain.Contains(type))
                return;
            refChain.Add(type);

            foreach (var reference in entry.References)
            {
                if (!filter(reference.Metadata.ClrType))
                    continue;
                
                await reference.LoadAsync(token);
                await reference.TargetEntry.LoadReferencesRecursivelyAsync(filter, token, refChain);
            }

            foreach (var collection in entry.Collections)
            {
                if (!filter(collection.Metadata.ClrType))
                    continue;

                await collection.LoadAsync(token);

                var enumerator = collection.CurrentValue.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    await collection.FindEntry(enumerator.Current).LoadReferencesRecursivelyAsync(filter, token, refChain);
                }
            }
        }
    }
}
