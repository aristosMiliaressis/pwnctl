using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using pwnctl.app.Assets.Entities;
using pwnctl.app.Common.ValueObjects;
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
                    new ScopeDefinitionAggregate(aggregate, new ScopeDefinition(ScopeType.CIDR, "172.16.17.0/24"))
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
                                    .Include(p => p.TaskProfiles)
                                    .ThenInclude(p => p.TaskProfile)
                                    .ThenInclude(p => p.TaskDefinitions)
                                    .First();

                var policy = new Policy(context.TaskProfiles.Include(p => p.TaskDefinitions).ToList());
                policy.Blacklist = "subfinder";
                context.Add(policy);
                //context.Entry(policy.TaskProfiles.First()).State = EntityState.Unchanged;
                context.SaveChanges();

                return policy;
            }
        }
        public static Operation Operation
        {
            get
            {
                PwnctlDbContext context = new();
                if (context.Operations.Any())
                    return context.Operations.First();

                var operation = new Operation("test", OperationType.Crawl, Policy, ScopeAggregate);
                context.Entry(operation.Scope).State = EntityState.Unchanged;
                context.Entry(operation.Policy).State = EntityState.Unchanged;
                context.Entry(operation.Policy.TaskProfiles.First()).State = EntityState.Unchanged;
                context.Add(operation);
                context.SaveChanges();

                return operation;
            }
        }


        public static TaskRecord TaskRecord
        {
            get
            {
                PwnctlDbContext context = new();
                if (context.TaskRecords.Any(t => t.Definition.Name == ShortName.Create("shortname_scanner")))
                    return context.TaskRecords
                                    .Include(t => t.Definition)
                                    .Include(t => t.Operation)
                                        .ThenInclude(o => o.Policy)
                                        .ThenInclude(o => o.TaskProfiles)
                                        .ThenInclude(o => o.TaskProfile)
                                        .ThenInclude(o => o.TaskDefinitions)
                                    .Include(t => t.Operation)
                                        .ThenInclude(o => o.Scope)
                                        .ThenInclude(o => o.Definitions)
                                        .ThenInclude(o => o.Definition)
                                    .First(t => t.Definition.Name == ShortName.Create("shortname_scanner"));

                

                var assetRecord = new AssetRecord(new DomainName("dummy.com"));

                var task = new TaskRecord(Operation, Policy.TaskProfiles.SelectMany(p => p.TaskProfile.TaskDefinitions).First(t => t.Name == ShortName.Create("shortname_scanner")), assetRecord);
                context = new();
                context.Entry(task.Definition).State = EntityState.Unchanged;
                context.Add(task);
                context.SaveChanges();

                task.Operation = Operation;

                return task;
            }
        }
    }
}
