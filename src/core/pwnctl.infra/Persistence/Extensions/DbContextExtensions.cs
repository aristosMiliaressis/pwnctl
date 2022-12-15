using Microsoft.EntityFrameworkCore;
using pwnctl.domain.BaseClasses;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Scope.Entities;
using System.Reflection;
using System.Linq.Expressions;
using pwnctl.domain.Entities;
using pwnctl.app.Common.Extensions;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.infra.Persistence.Extensions
{
    public static class DbContextExtensions
    {        
        public static List<Program> ListPrograms(this PwnctlDbContext context)
        {
            return context.Programs
                            .Include(p => p.Policy)
                            .Include(p => p.Scope)
                            .AsNoTracking()
                            .ToList();
        }

        public static IQueryable<TaskRecord> JoinedTaskRecordQueryable(this PwnctlDbContext context)
        {
            return context.TaskRecords
                            .Include(r => r.Definition)
                            .Include(r => r.Host)
                            .Include(r => r.Domain)
                            .Include(r => r.Endpoint)
                            .Include(r => r.NetRange)
                            .Include(r => r.DNSRecord)
                            .Include(r => r.Service)
                            .Include(r => r.CloudService)
                            .Include(r => r.Keyword);
        }

        public static Asset FindAsset(this DbContext context, Asset asset)
        {
            var lambda = ExpressionTreeBuilder.BuildAssetMatchingLambda(asset);

            return (Asset)context.FirstFromLambda(lambda);
        }

        public static Tag FindAssetTag(this DbContext context, Asset asset, Tag tag)
        {
            var lambda = ExpressionTreeBuilder.BuildTagMatchingLambda(asset, tag);
            return (Tag)context.FirstFromLambda(lambda);
        }

        public static Entity FirstFromLambda(this DbContext context, LambdaExpression lambda)
        {
            var type = lambda.Parameters.First().Type;

            var dbSetMethod = _dbSetMethod.MakeGenericMethod(type);
            var queryableDbSet = dbSetMethod.Invoke(context, null);

            var whereMethod = _whereMethod.MakeGenericMethod(type);

            var filteredQueryable = whereMethod.Invoke(null, new object[] { queryableDbSet, lambda });

            var firstOrDefaultMethod = _firstOrDefaultMethod.MakeGenericMethod(type);
            return (Entity)firstOrDefaultMethod.Invoke(null, new object[] { filteredQueryable });
        }

        public static Entity FirstNotTrackedFromLambda(this DbContext context, LambdaExpression lambda)
        {
            var type = lambda.Parameters.First().Type;

            var dbSetMethod = _dbSetMethod.MakeGenericMethod(type);
            var queryableDbSet = dbSetMethod.Invoke(context, null);

            var whereMethod = _whereMethod.MakeGenericMethod(type);

            var filteredQueryable = whereMethod.Invoke(null, new object[] { queryableDbSet, lambda });

            var asNoTrackingMethod = _asNoTrackingMethod.MakeGenericMethod(type);
            filteredQueryable = asNoTrackingMethod.Invoke(null, new object[] { filteredQueryable });

            var firstOrDefaultMethod = _firstOrDefaultMethod.MakeGenericMethod(type);
            return (Entity)firstOrDefaultMethod.Invoke(null, new object[] { filteredQueryable });
        }

        public static void ConvertDateTimesToUtc(this DbContext context)
        {
            var dateProperties = context.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(DateTime))
                .Select(z => new
                {
                    ParentName = z.DeclaringEntityType.Name,
                    PropertyName = z.Name
                });

            var editedEntitiesInTheDbContextGraph = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(x => x.Entity);

            foreach (var entity in editedEntitiesInTheDbContextGraph)
            {
                var entityFields = dateProperties.Where(d => d.ParentName == entity.GetType().FullName);

                foreach (var property in entityFields)
                {
                    var prop = entity.GetType().GetProperty(property.PropertyName);

                    if (prop == null)
                        continue;

                    var originalValue = prop.GetValue(entity) as DateTime?;
                    if (originalValue == null)
                        continue;

                    if (originalValue.Value.Kind == DateTimeKind.Local)
                        prop.SetValue(entity, TimeZoneInfo.ConvertTimeToUtc(originalValue.Value, TimeZoneInfo.Local));
                }
            }
        }

        private static MethodInfo _dbSetMethod = typeof(PwnctlDbContext).GetMethod(nameof(PwnctlDbContext.Set), BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), null);
        private static MethodInfo _whereMethod = typeof(Queryable).GetMethods().Where(m => m.Name == nameof(Queryable.Where)).First();
        private static MethodInfo _firstOrDefaultMethod = typeof(Queryable).GetMethods().Where(m => m.Name == nameof(Queryable.FirstOrDefault)).First();
        private static MethodInfo _asNoTrackingMethod = typeof(EntityFrameworkQueryableExtensions).GetMethods().Where(m => m.Name == nameof(EntityFrameworkQueryableExtensions.AsNoTracking)).First();
    }
}
