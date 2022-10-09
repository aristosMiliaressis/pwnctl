using pwnwrk.domain.Attributes;
using pwnwrk.domain.BaseClasses;
using pwnwrk.domain.Entities;
using System.Reflection;
using System.Linq.Expressions;

namespace pwnwrk.infra
{
    public static class ExpressionTreeBuilder
    {
        public static LambdaExpression BuildAssetMatchingLambda(BaseAsset asset)
        {
            var type = asset.GetType();

            var uniqnessProperties = type.GetProperties().Where(p => p.GetCustomAttribute(typeof(UniquenessAttribute)) != null);

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

        public static LambdaExpression BuildTaskMatchingLambda(BaseAsset asset, TaskDefinition definition)
        {
            var type = typeof(domain.Entities.Task);

            var _param = Expression.Parameter(type, "t");

            var lref = Expression.PropertyOrField(_param, nameof(domain.Entities.Task.DefinitionId));
            var rval = Expression.Constant(definition.Id);
            var expression = Expression.Equal(lref, rval);

            lref = Expression.PropertyOrField(_param, asset.GetType().Name + "Id");
            rval = Expression.Constant(asset.Id);
            var assetExpression = Expression.Equal(lref, rval);

            expression = Expression.AndAlso(expression, assetExpression);

            var lamdaMethod = _lambdaMethod.MakeGenericMethod(typeof(Func<,>).MakeGenericType(type, typeof(bool)));
            var lambda = (LambdaExpression)lamdaMethod.Invoke(null, new object[] { expression, new ParameterExpression[] { _param } });

            return lambda;
        }

        public static LambdaExpression BuildTagMatchingLambda(BaseAsset asset, Tag tag)
        {
            var type = typeof(Tag);

            var _param = Expression.Parameter(type, "t");

            var lref = Expression.PropertyOrField(_param, nameof(Tag.Name));
            var rval = Expression.Constant(tag.Name);
            var expression = Expression.Equal(lref, rval);

            lref = Expression.PropertyOrField(_param, asset.GetType().Name + "Id");
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