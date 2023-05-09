using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Operations.Enums;
using pwnctl.app.Scope.Entities;
using pwnctl.app.Scope.Enums;
using pwnctl.app.Tasks.Entities;
using pwnctl.domain.Entities;
using pwnctl.infra.Persistence;

namespace pwnctl.app.test.unit
{
    public static class EntityFactory
    {
        public static ScopeAggregate ScopeAggregate
        {
            get
            {
                PwnctlDbContext context = new();
                if (context.ScopeAggregates.Any())
                    return context.ScopeAggregates
                                    .Include(o => o.Definitions)
                                        .ThenInclude(o => o.Definition)
                                    .First();

                var aggregate = new ScopeAggregate("test", "");

                aggregate.Definitions = new List<ScopeDefinitionAggregate>
                {
                    new ScopeDefinitionAggregate(aggregate, new ScopeDefinition(ScopeType.DomainRegex, "(^tesla\\.com$|.*\\.tesla\\.com$)")),
                    new ScopeDefinitionAggregate(aggregate, new ScopeDefinition(ScopeType.UrlRegex, "(.*:\\/\\/tsl\\.com\\/app\\/.*$)")),
                    new ScopeDefinitionAggregate(aggregate, new ScopeDefinition(Scope.Enums.ScopeType.CIDR, "172.16.17.0/24"))
                };

                context.Add(aggregate);
                context.SaveChanges();

                return aggregate;
            }
        }

        public static Policy Policy
        {
            get
            {
                PwnctlDbContext context = new();

                if (context.Policies.Any())
                    return context.Policies
                                    .Include(p => p.TaskProfile)
                                    .ThenInclude(p => p.TaskDefinitions)
                                    .First();

                var policy = new Policy(context.TaskProfiles.Include(p => p.TaskDefinitions).First());
                policy.Whitelist = "ffuf_common";
                policy.Blacklist = "subfinder";
                policy.MaxAggressiveness = 6;
                context.Entry(policy.TaskProfile).State = EntityState.Unchanged;
                context.Add(policy);
                context.SaveChanges();

                return policy;
            }
        }


        public static TaskEntry TaskEntry
        {
            get
            {
                PwnctlDbContext context = new();
                if (context.TaskEntries.Any())
                    return context.TaskEntries
                                    .Include(t => t.Operation)
                                        .ThenInclude(o => o.Policy)
                                        .ThenInclude(o => o.TaskProfile)
                                        .ThenInclude(o => o.TaskDefinitions)
                                    .Include(t => t.Operation)
                                        .ThenInclude(o => o.Scope)
                                        .ThenInclude(o => o.Definitions)
                                        .ThenInclude(o => o.Definition)
                                    .First();

                var operation = new Operation("test", OperationType.Crawl, Policy, ScopeAggregate);
                context.Entry(operation.Scope).State = EntityState.Unchanged;
                context.Entry(operation.Policy).State = EntityState.Unchanged;
                context.Entry(operation.Policy.TaskProfile).State = EntityState.Unchanged;
                context.Add(operation);
                context.SaveChanges();

                var assetRecord = new AssetRecord(new DomainName("dummy.com"));

                var task = new TaskEntry(operation, Policy.TaskProfile.TaskDefinitions.First(), assetRecord);
                context = new();
                context.Entry(task.Definition).State = EntityState.Unchanged;
                context.Entry(task.Operation).State = EntityState.Unchanged;
                context.Add(task);
                context.SaveChanges();

                return task;
            }
        }
    }
}
