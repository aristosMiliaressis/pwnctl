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

namespace pwnctl.core.test.integration.Helpers
{
    public static class EntityFactory
    {
        public static Operation EnsureMonitorOperationCreated()
        {
            PwnctlDbContext context = new();
            
            var op = context.Operations.FirstOrDefault(a => a.Name == ShortName.Create("monitor_tesla"));
            if (op is not null)
            {
                return op;
            }

            var scope = context.ScopeAggregates.FirstOrDefault(a => a.Name == ShortName.Create("tesla_scope"));
            if (scope == null)
            {
                scope = new ScopeAggregate("tesla_scope", "");
                scope.Definitions = new List<ScopeDefinitionAggregate>
                {
                    new ScopeDefinitionAggregate(scope, new ScopeDefinition(ScopeType.DomainRegex, "(^tesla\\.com$|.*\\.tesla\\.com$)")),
                    new ScopeDefinitionAggregate(scope, new ScopeDefinition(ScopeType.UrlRegex, "(.*:\\/\\/tsl\\.com\\/app\\/.*$)")),
                    new ScopeDefinitionAggregate(scope, new ScopeDefinition(ScopeType.CIDR, "172.16.17.0/24"))
                };
            }
            var taskProfiles = context.TaskProfiles.Include(p => p.TaskDefinitions).ToList();
            var policy = new Policy(taskProfiles);
            op = new Operation("monitor_tesla", OperationType.Monitor, policy, scope);
            context.Add(op);
            context.SaveChanges();
            var domain = DomainName.TryParse("tesla.com").Value;
            var record = new AssetRecord(domain);
            record.SetScopeId(scope.Definitions.First().Definition.Id);
            context.Add(record);
            context.SaveChanges();

            return op;
        }

        public static Operation EnsureCrawlOperationCreated()
        {
            PwnctlDbContext context = new();
            var op = context.Operations.FirstOrDefault(a => a.Name == ShortName.Create("crawl_starlink"));
            if (op is not null)
            {
                return op;
            }

            var scope = new ScopeAggregate("starlink_scope", "");
            scope.Definitions = new List<ScopeDefinitionAggregate>
                {
                    new ScopeDefinitionAggregate(scope, new ScopeDefinition(ScopeType.DomainRegex, "(^starlink\\.com$|.*\\.starlink\\.com$)")),
                    new ScopeDefinitionAggregate(scope, new ScopeDefinition(ScopeType.CIDR, "172.16.17.0/24"))
                };
            var taskProfiles = context.TaskProfiles.Include(p => p.TaskDefinitions).ToList();
            var policy = new Policy(taskProfiles);
            op = new Operation("crawl_starlink", OperationType.Crawl, policy, scope);
            context.Add(op);
            context.SaveChanges();
            var domain = DomainName.TryParse("starlink.com").Value;
            var record = new AssetRecord(domain);
            record.SetScopeId(scope.Definitions.First().Definition.Id);
            context.Add(record);
            context.SaveChanges();

            return op;
        }
    }
}
