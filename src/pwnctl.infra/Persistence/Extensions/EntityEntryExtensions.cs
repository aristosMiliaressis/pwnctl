using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace pwnctl.infra.Persistence.Extensions
{
    public static class EntityEntryExtensions
    {
        public static async Task LoadReferencesRecursivelyAsync(this EntityEntry entry, CancellationToken token = default, List<Type> refChain = null)
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

            if (entry.References.Any())
            {
                foreach (var nestedReference in entry.References)
                {
                    await nestedReference.LoadAsync(token);
                    await nestedReference.TargetEntry.LoadReferencesRecursivelyAsync(token, refChain);
                }
            }

            if (entry.Collections.Any())
            {
                foreach (var nestedCollection in entry.Collections)
                {
                    await nestedCollection.LoadAsync(token);
                    var enumerator = nestedCollection.CurrentValue.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        await nestedCollection.FindEntry(enumerator.Current).LoadReferencesRecursivelyAsync(token, refChain);
                    }
                }
            }
        }
    }
}
