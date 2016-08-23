using System;
using System.Linq.Expressions;

namespace Oedis
{
    public static class ExpressionTreeHelpers
    {
        internal static bool IsMemberEqualsValueExpression(this Expression exp, Type declaringType, string memberName)
        {
            if (exp.NodeType != ExpressionType.Equal)
                return false;

            BinaryExpression be = (BinaryExpression)exp;

            // Assert.
            if (be.Left.IsSpecificMemberExpression(declaringType, memberName) &&
                be.Right.IsSpecificMemberExpression(declaringType, memberName))
                throw new Exception("Cannot have 'member' == 'member' in an expression!");

            return (be.Left.IsSpecificMemberExpression(declaringType, memberName) ||
                be.Right.IsSpecificMemberExpression(declaringType, memberName));
        }

        internal static bool IsSpecificMemberExpression(this Expression exp, Type declaringType, string memberName)
        {
            return ((exp is MemberExpression) &&
                (((MemberExpression)exp).Member.DeclaringType == declaringType) &&
                (((MemberExpression)exp).Member.Name == memberName));
        }

        public static string GetValue(this Expression exp, Type memberDeclaringType, string memberName)
        {
            BinaryExpression be = (BinaryExpression)exp;
            if (be.NodeType != ExpressionType.Equal)
                throw new Exception("There is a bug in this program.");

            if (be.Left.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression me = (MemberExpression)be.Left;

                if (me.Member.DeclaringType == memberDeclaringType && me.Member.Name == memberName)
                {
                    return be.Right.GetExpressionValue();
                }
            }
            else if (be.Right.NodeType == ExpressionType.MemberAccess)
            {
                MemberExpression me = (MemberExpression)be.Right;

                if (me.Member.DeclaringType == memberDeclaringType && me.Member.Name == memberName)
                {
                    return be.Left.GetExpressionValue();
                }
            }

            // We should have returned by now.
            throw new Exception("There is a bug in this program.");
        }

        private static string GetExpressionValue(this Expression expression)
        {
            if (expression.NodeType == ExpressionType.MemberAccess || expression.NodeType == ExpressionType.New)
            {
                return MemberExpression.Lambda(expression, new ParameterExpression[] { }).Compile().DynamicInvoke(new object[] { }).ToString();
            }
            if (expression.NodeType == ExpressionType.Call)
            {
                return ((MethodCallExpression)expression).Arguments[0].GetExpressionValue();
            }
            if (expression.NodeType == ExpressionType.Constant)
                return (((ConstantExpression)expression).Value).ToString();
            else
                throw new Exception(
                    String.Format("The expression type {0} is not supported to obtain a value.", expression.NodeType));
        }
    }
}