using pwnctl.kernel.Attributes;
using pwnctl.domain.BaseClasses;
using pwnctl.app.Tasks.Entities;
using System.Reflection;
using System.Linq.Expressions;
using pwnctl.app.Tagging.Entities;

namespace pwnctl.app.Common.Extensions
{
    public static class ExpressionTreeBuilder
    {
        /// <summary>
        /// builds a LambdaExpression object that matches the `asset` parameter
        /// </summary>
        public static LambdaExpression BuildAssetMatchingLambda(Asset asset)
        {
            var type = asset.GetType();

            var uniqnessProperties = type.GetProperties().Where(p => p.GetCustomAttribute(typeof(EqualityComponentAttribute)) != null);

            var _param = Expression.Parameter(type, "e");

            Expression expression = null;
            foreach (var property in uniqnessProperties)
            {
                var lref = Expression.PropertyOrField(_param, property.Name);
                var rval = Expression.Constant(property.GetValue(asset));
                var propertyExpression = Expression.Equal(lref, rval);

                expression = (expression == null)
                    ? propertyExpression
                    : Expression.AndAlso(expression, propertyExpression);
            }

            var lamdaMethod = _lambdaMethod.MakeGenericMethod(typeof(Func<,>).MakeGenericType(type, typeof(bool)));
            var lambda = (LambdaExpression) lamdaMethod.Invoke(null, new object[] { expression, new ParameterExpression[] { _param } });

            return lambda;
        }

        /// <summary>
        /// builds a LambdaExpression object that matches a task of the provided TaskDefinition that has been queued against the provided asset.
        /// </summary>
        public static LambdaExpression BuildTaskMatchingLambda(Asset asset, TaskDefinition definition)
        {
            var type = typeof(TaskEntry);

            var _param = Expression.Parameter(type, "t");

            var lref = Expression.PropertyOrField(_param, nameof(TaskEntry.DefinitionId));
            var rval = Expression.Constant(definition.Id);
            var expression = Expression.Equal(lref, rval);

            lref = Expression.PropertyOrField(_param, nameof(Tag.RecordId));
            rval = Expression.Constant(asset.Id);
            var assetExpression = Expression.Equal(lref, rval);

            expression = Expression.AndAlso(expression, assetExpression);

            var lamdaMethod = _lambdaMethod.MakeGenericMethod(typeof(Func<,>).MakeGenericType(type, typeof(bool)));
            var lambda = (LambdaExpression)lamdaMethod.Invoke(null, new object[] { expression, new ParameterExpression[] { _param } });

            return lambda;
        }

        private static MethodInfo _lambdaMethod = typeof(Expression).GetMethods().Where(m => m.Name == nameof(Expression.Lambda)
                        && m.IsGenericMethod && m.GetParameters().Count() == 2
                        && m.GetParameters()[1].ParameterType == typeof(ParameterExpression[])).First();
    }    
}