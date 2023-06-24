using System.Linq.Expressions;
using System.Text;

namespace MudExtensions
{
    public class PrettyPrintExpressionVisitor : ExpressionVisitor
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public string GetExpression(Expression expression)
        {
            this.Visit(expression);
            return _stringBuilder.ToString();
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            _stringBuilder.Append("(");
            this.Visit(node.Left);
            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                    _stringBuilder.Append("\nEqual ");
                    break;
                case ExpressionType.AndAlso:
                    _stringBuilder.Append("\nAnd ");
                    break;
                case ExpressionType.OrElse:
                    _stringBuilder.Append("\nOr ");
                    break;
                case ExpressionType.NotEqual:
                    _stringBuilder.Append("\nNotEqual ");
                    break;
                default:
                    _stringBuilder.Append("\n" + node.NodeType);
                    break;
            }
            this.Visit(node.Right);
            _stringBuilder.Append("\n)");
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _stringBuilder.Append(node.Value + "\n");
            return node;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            _stringBuilder.Append(node.Name + "\n");
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            _stringBuilder.Append(node.Method.Name + "(");
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                this.Visit(node.Arguments[i]);
                if (i < node.Arguments.Count - 1)
                {
                    _stringBuilder.Append(", ");
                }
            }
            _stringBuilder.Append(")\n");
            return node;
        }
    }


    public class CompactExpressionPrinter : ExpressionVisitor
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public string GetExpression(Expression expression)
        {
            this.Visit(expression);
            return _stringBuilder.ToString();
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            _stringBuilder.Append("(");
            this.Visit(node.Left);
            _stringBuilder.Append(node.NodeType.ToString().Replace("AndAlso", "And").Replace("OrElse", "Or"));
            this.Visit(node.Right);
            _stringBuilder.Append(")");
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _stringBuilder.Append(node.Value);
            return node;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            _stringBuilder.Append(node.Name);
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            _stringBuilder.Append(node.Method.Name + "(");
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                this.Visit(node.Arguments[i]);
                if (i < node.Arguments.Count - 1)
                {
                    _stringBuilder.Append(",");
                }
            }
            _stringBuilder.Append(")");
            return node;
        }
    }



}
