// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace MudExtensions
{
#nullable enable
    /// <summary>
    /// Helper class to perform different operations on Expressions
    /// </summary>
    internal static class ExpressionModifier
    {
        /// <summary>
        /// Modifies firstExpression by replacing the parameter in the secondExpression
        /// </summary>
        internal static Expression<Func<T, bool>> Modify<T>(this Expression firstExpression, Expression secondExpression)
        {
            var bodyIdentifier = new ExpressionBodyVisitor();
            var body = bodyIdentifier.ExtractBody(firstExpression);
            var parameterIdentifier = new ExpressionParameterVisitor();
            var parameter = (ParameterExpression)parameterIdentifier.ExtractParameter(firstExpression);
            var body2 = bodyIdentifier.ExtractBody(secondExpression);
            var parameter2 = (ParameterExpression)parameterIdentifier.ExtractParameter(secondExpression);

            var treeModifier = new ExpressionReplacer(parameter2, body);
            return Expression.Lambda<Func<T, bool>>(treeModifier.Visit(body2), parameter);
        }

        /// <summary>
        /// Replaces all binary expressions in the given expression that match a certain type with another type
        /// </summary>
        internal static Expression ReplaceBinary(this Expression exp, ExpressionType from, ExpressionType to)
        {
            var binaryReplacer = new BinaryReplacer(from, to);
            return binaryReplacer.Visit(exp);
        }

        /// <summary>
        /// Creates a binary expression of a given type from the provided expression and a constant value
        /// </summary>
        public static Expression<Func<T, bool>> GenerateBinary<T>(this Expression expression, ExpressionType binaryOperation, object? value)
        {
            var bodyIdentifier = new ExpressionBodyVisitor();
            var body = bodyIdentifier.ExtractBody(expression);
            var parameterIdentifier = new ExpressionParameterVisitor();
            var parameter = (ParameterExpression)parameterIdentifier.ExtractParameter(expression);

            // Handle nullable types
            if (Nullable.GetUnderlyingType(body.Type) is not null && value is null)
            {
                // property type is nullable and value is null...
                var binaryExpression = Expression.MakeBinary(binaryOperation, body, Expression.Constant(value, body.Type));
                return Expression.Lambda<Func<T, bool>>(binaryExpression, parameter);
            }

            // Short-circuit if the property type is not nullable and the value is null
            if (value is null)
            {
                // We can short circuit here because the value to be compared is null and the property type is not nullable.
                return x => true;
            }

            // Property type is not nullable and value is not null
            var nonNullableBinaryExpression = Expression.MakeBinary(binaryOperation, body, Expression.Constant(value));
            return Expression.Lambda<Func<T, bool>>(nonNullableBinaryExpression, parameter);
        }

        /// <summary>
        /// Changes the return type of the provided expression
        /// </summary>
        public static Expression<Func<T, U>> ChangeExpressionReturnType<T, U>(this Expression expression)
        {
            var bodyIdentifier = new ExpressionBodyVisitor();
            var body = bodyIdentifier.ExtractBody(expression);
            var parameterIdentifier = new ExpressionParameterVisitor();
            var parameter = (ParameterExpression)parameterIdentifier.ExtractParameter(expression);

            // If the expression already has the right type, return it
            if (body.Type == typeof(U))
            {
                return Expression.Lambda<Func<T, U>>(body, parameter);
            }

            // If not, convert the expression body to the target type
            var converted = Expression.Convert(body, typeof(U));
            return Expression.Lambda<Func<T, U>>(converted, parameter);
        }
    }

    /// <summary>
    /// Visitor to replace one expression with another in an expression tree
    /// </summary>
    internal class ExpressionReplacer : ExpressionVisitor
    {
        private readonly Expression _from;
        private readonly Expression _to;

        public ExpressionReplacer(Expression from, Expression to)
        {
            _from = from;
            _to = to;
        }

        [return: NotNullIfNotNull(nameof(node))]
        public override Expression? Visit(Expression? node)
        {
            if (node == _from) return _to;
            return base.Visit(node);
        }
    }

    /// <summary>
    /// Visitor to extract the body of a Lambda Expression
    /// </summary>
    internal class ExpressionBodyVisitor : ExpressionVisitor
    {
        public Expression ExtractBody(Expression node)
        {
            return base.Visit(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return node.Body;
        }
    }

    /// <summary>
    /// Visitor to extract the first parameter of a Lambda Expression
    /// </summary>
    internal class ExpressionParameterVisitor : ExpressionVisitor
    {
        public Expression ExtractParameter(Expression node)
        {
            if (node is LambdaExpression lambdaExpression && lambdaExpression.Parameters.Count > 0)
            {
                return lambdaExpression.Parameters[0];
            }
            else
            {
                throw new InvalidOperationException("No parameters found in the provided expression");
            }
        }
    }

    /// <summary>
    /// Visitor to replace a certain type of binary expression with another in an expression tree
    /// </summary>
    internal class BinaryReplacer : ExpressionVisitor
    {
        private readonly ExpressionType _from;
        private readonly ExpressionType _to;

        public BinaryReplacer(ExpressionType from, ExpressionType to)
        {
            _from = from;
            _to = to;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == _from)
            {
                return Expression.MakeBinary(_to, Visit(node.Left), Visit(node.Right));
            }

            return base.VisitBinary(node);
        }
    }
}
