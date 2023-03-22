using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using pwnctl.domain.BaseClasses;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.Extensions
{
    public static class EntityEntryExtensions
    {
        public static async Task LoadReferenceGraphAsync(this EntityEntry entry, CancellationToken token = default, List<string> refChain = null)
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
                await reference.TargetEntry.LoadReferenceGraphAsync(token, refChain);
            }
            
            entry.State = EntityState.Unchanged;

            foreach (var collection in entry.Collections)
            {
                await collection.LoadAsync(token);

                var enumerator = collection.CurrentValue.GetEnumerator();
                var elements = new List<EntityEntry>();
                while (enumerator.MoveNext())
                {
                    var element = collection.FindEntry(enumerator.Current);
                    element.State = EntityState.Detached;
                    elements.Add(element);
                }
                foreach (var element in elements.DistinctBy(e => e.Entity.ToString()))
                {
                    await element.LoadReferenceGraphAsync(token, refChain);
                }
            }

            entry.State = EntityState.Detached;
        }

        public static void DetachReferenceGraph(this EntityEntry entry, List<string> refChain = null)
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
                reference.TargetEntry.DetachReferenceGraph(refChain);
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
