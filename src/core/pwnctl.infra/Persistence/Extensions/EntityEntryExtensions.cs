using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace pwnctl.infra.Persistence.Extensions
{
    public static class EntityEntryExtensions
    {
        public static async Task LoadReferencesRecursivelyAsync(this EntityEntry entry, CancellationToken token = default, List<string> refChain = null)
        {
            if (entry == null)
                return;

            // if reference chain is null(i.e recursive entry point) initialize the chain
            // else create a copy, to mentain a separet chain between every reqursive branch
            refChain = refChain == null
                    ? new List<string>()
                    : new List<string>(refChain);

            // if type exists in chain return to prevent infinit loop
            if (refChain.Contains(entry.Entity.ToString()))
                return;
            refChain.Add(entry.Entity.ToString());

            entry.State = EntityState.Unchanged;

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

            entry.State = EntityState.Detached;
        }

        public static void DetachReferechGraph(this EntityEntry entry, List<string> refChain = null)
        {
            if (entry == null)
                return;

            // if reference chain is null(i.e recursive entry point) initialize the chain
            // else create a copy, to mentain a separet chain between every reqursive branch
            refChain = refChain == null
                    ? new List<string>()
                    : new List<string>(refChain);

            // if type exists in chain return to prevent infinit loop
            if (refChain.Contains(entry.Entity.ToString()))
                return;
            refChain.Add(entry.Entity.ToString());

            foreach (var reference in entry.References.Where(r => r.TargetEntry != null))
            {
                reference.TargetEntry.DetachReferechGraph(refChain);
            }

            foreach (var collection in entry.Collections)
            {
                var enumerator = collection?.CurrentValue?.GetEnumerator();
                if (enumerator == null)
                    continue;
                    
                while (enumerator.MoveNext())
                {
                    var colEntry = collection.FindEntry(enumerator.Current);
                    if (colEntry != null)
                    {
                        colEntry.State = EntityState.Detached;
                    }
                }
            }

            entry.State = EntityState.Detached;
        }
    }
}
