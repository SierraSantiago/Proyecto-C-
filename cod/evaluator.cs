using System;
using System.Diagnostics;

namespace cod
{
    public class Evaluator
    {
        Object TRUE = new Boolean(true);
        Object FALSE = new Boolean(false);
        Object NULL = new Null();

        public Object EvaluateBangOperatorExpression(Object right)
        {
            if (right == TRUE)
            {
                return FALSE;
            }
            else if (right == FALSE || right == NULL)
            {
                return TRUE;
            }
            else
            {
                return FALSE;
            }
        }
        public Boolean ToBooleanObject(bool value)
        {
            return value ? (Boolean)TRUE : (Boolean)FALSE;
        }
        public Object EvaluateMinusOperatorExpression(Object right)
        {
            if (right.Type != ObjectType.INTEGER)
            {
                return NULL;
            }

            Inte integer = (Inte)right;
            return new Inte(-integer.Value);
        }
        public Object EvaluatePrefixExpression(string @operator, Object right)
        {
            if (@operator == "!")
            {
                return EvaluateBangOperatorExpression(right);
            }
            else if (@operator == "-")
            {
                return EvaluateMinusOperatorExpression(right);
            }
            else
            {
                return NULL;
            }
        }

        public Object EvaluateIntegerInfixExpression(string @operator, Object left, Object right)
        {
            int leftValue = ((Inte)left).Value;
            int rightValue = ((Inte)right).Value;

            if (@operator == "+")
            {
                return new Inte(leftValue + rightValue);
            }
            else if (@operator == "-")
            {
                return new Inte(leftValue - rightValue);
            }
            else if (@operator == "*")
            {
                return new Inte(leftValue * rightValue);
            }
            else if (@operator == "/")
            {
                return new Inte(leftValue / rightValue);
            }
            else if (@operator == "<")
            {
                return ToBooleanObject(leftValue < rightValue);
            }
            else if (@operator == "<=")
            {
                return ToBooleanObject(leftValue <= rightValue);
            }
            else if (@operator == ">")
            {
                return ToBooleanObject(leftValue > rightValue);
            }
            else if (@operator == ">=")
            {
                return ToBooleanObject(leftValue >= rightValue);
            }
            else if (@operator == "==")
            {
                return ToBooleanObject(leftValue == rightValue);
            }
            else if (@operator == "!=")
            {
                return ToBooleanObject(leftValue != rightValue);
            }

            return NULL;
        }
        public Object EvaluateInfixExpression(string @operator, Object left, Object right)
        {
            if (left.Type == ObjectType.INTEGER && right.Type == ObjectType.INTEGER)
            {
                return EvaluateIntegerInfixExpression(@operator, left, right);
            }
            else if (@operator == "==")
            {
                return ToBooleanObject(left == right);
            }
            else if (@operator == "!=")
            {
                return ToBooleanObject(left != right);
            }

            return NULL;
        }
        public Object Evaluate(ASTNode node)
        {
            Type nodeType = node.GetType();

            if (nodeType == typeof(Program))
            {
                Program program = (Program)node;
                return EvaluateStatements(program.Statements);
            }
            else if (nodeType == typeof(ExpressionStatement))
            {
                ExpressionStatement expressionStatement = (ExpressionStatement)node;
                Debug.Assert(expressionStatement.Expression != null);
                return Evaluate(expressionStatement.Expression);
            }
            else if (nodeType == typeof(Integer))
            {
                Integer integer = (Integer)node;
                Debug.Assert(integer.Value.HasValue);
                return new Inte(integer.Value.Value);
            }
            else if (nodeType == typeof(Boolea))
            {
                Boolea boolean = (Boolea)node;
                Debug.Assert(boolean.Value.HasValue);
                return ToBooleanObject(boolean.Value.Value);
            }
            else if (nodeType == typeof(ParsePrefixExpression))
            {
                ParsePrefixExpression prefixExpression = (ParsePrefixExpression)node;
                Debug.Assert(prefixExpression.Right != null);
                Object right = Evaluate(prefixExpression.Right);
                Debug.Assert(right != null);
                return EvaluatePrefixExpression(prefixExpression.Operator, right);
            }
            else if (nodeType == typeof(Infix))
            {
                Infix infixExpression = (Infix)node;
                Debug.Assert(infixExpression.Left != null && infixExpression.Right != null);
                var left = Evaluate(infixExpression.Left);
                var right = Evaluate(infixExpression.Right);
                Debug.Assert(left != null && right != null);
                return EvaluateInfixExpression(infixExpression.Operator, left, right);
            }
            else if (nodeType == typeof(Block))
            {
                Block block = (Block)node;
                return EvaluateStatements(block.Statements);
            }
            else if (nodeType == typeof(If))
            {
                If ifExpression = (If)node;
                return EvaluateIfExpression(ifExpression);
            }

            return null;
        }

        public Object EvaluateStatements(List<Statement> statements)
        {
            Object result = null;

            foreach (var Statement in statements)
            {
                result = Evaluate(Statement);
            }

            return result;
        }

        private bool IsTruthy(Object obj)
        {
            if (obj == NULL)
            {
                return false;
            }
            else if (obj == TRUE)
            {
                return true;
            }
            else if (obj == FALSE)
            {
                return false;
            }
            return true;

        }
        private Object EvaluateIfExpression(If ifExpression)
        {
            Debug.Assert(ifExpression != null);

            Object condition = Evaluate(ifExpression.Condition);
            Debug.Assert(condition != null);

            if (IsTruthy(condition))
            {
                Debug.Assert(ifExpression.Consequence != null);
                return Evaluate(ifExpression.Consequence);
            }
            else if (ifExpression.Alternative != null)
            {
                return Evaluate(ifExpression.Alternative);
            }
            return NULL;
        }


    }

}