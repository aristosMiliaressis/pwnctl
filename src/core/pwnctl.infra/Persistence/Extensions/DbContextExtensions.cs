using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Linq.Expressions;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.infra.Persistence.Extensions
{
    public static class DbContextExtensions
    {
        public static TEntity FirstFromLambda<TEntity>(this DbContext context, LambdaExpression lambda)
        {
            var type = lambda.Parameters.First().Type;

            var dbSetMethod = _dbSetMethod.MakeGenericMethod(type);
            var queryableDbSet = dbSetMethod.Invoke(context, null);

            var whereMethod = _whereMethod.MakeGenericMethod(type);

            var filteredQueryable = whereMethod.Invoke(null, new object[] { queryableDbSet, lambda });

            var firstOrDefaultMethod = _firstOrDefaultMethod.MakeGenericMethod(type);
            return (TEntity) firstOrDefaultMethod.Invoke(null, new object[] { filteredQueryable });
        }

        public static Task<TEntity> FirstNotTrackedFromLambdaAsync<TEntity>(this DbContext context, LambdaExpression lambda, CancellationToken token = default)
        {
            var type = lambda.Parameters.First().Type;

            var dbSetMethod = _dbSetMethod.MakeGenericMethod(type);
            var queryableDbSet = dbSetMethod.Invoke(context, null);

            var whereMethod = _whereMethod.MakeGenericMethod(type);

            var filteredQueryable = whereMethod.Invoke(null, new object[] { queryableDbSet, lambda });

            var asNoTrackingMethod = _asNoTrackingMethod.MakeGenericMethod(type);
            filteredQueryable = asNoTrackingMethod.Invoke(null, new object[] { filteredQueryable });

            var firstOrDefaultMethod = _firstOrDefaultAsyncMethod.MakeGenericMethod(type);
            return (Task<TEntity>)firstOrDefaultMethod.Invoke(null, new object[] { filteredQueryable, token });
        }
        
        private static MethodInfo _dbSetMethod = typeof(PwnctlDbContext).GetMethod(nameof(PwnctlDbContext.Set), BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), null);
        private static MethodInfo _whereMethod = typeof(Queryable).GetMethods().Where(m => m.Name == nameof(Queryable.Where)).First();
        private static MethodInfo _firstOrDefaultMethod = typeof(Queryable).GetMethods().Where(m => m.Name == nameof(Queryable.FirstOrDefault)).First();
        private static MethodInfo _firstOrDefaultAsyncMethod = typeof(EntityFrameworkQueryableExtensions).GetMethods().Where(m => m.Name == nameof(EntityFrameworkQueryableExtensions.FirstOrDefaultAsync)).First();
        private static MethodInfo _asNoTrackingMethod = typeof(EntityFrameworkQueryableExtensions).GetMethods().Where(m => m.Name == nameof(EntityFrameworkQueryableExtensions.AsNoTracking)).First();
    }
}
