using Microsoft.EntityFrameworkCore;
using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Common.Entities;
using pwnwrk.domain.Common.BaseClasses;
using pwnwrk.domain.Tasks.Entities;
using pwnwrk.domain.Targets.Entities;
using System.Reflection;
using System.Linq.Expressions;

namespace pwnwrk.infra.Persistence.Extensions
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

        public static Asset FindAsset(this DbContext context, Asset asset)
        {
            var lambda = ExpressionTreeBuilder.BuildAssetMatchingLambda(asset);

            return (Asset)context.FirstFromLambda(lambda);
        }

        public static TaskRecord FindAssetTaskRecord(this DbContext context, Asset asset, TaskDefinition def)
        {
            var lambda = ExpressionTreeBuilder.BuildTaskMatchingLambda(asset, def);
            return (TaskRecord)context.FirstFromLambda(lambda);
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
    }
}
