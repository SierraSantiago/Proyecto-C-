using System;
using System.Diagnostics;

namespace cod
{
    public class Evaluator
    {
        Object TRUE = new Boolean(true);
        Object FALSE = new Boolean(false);
        Object NULL = new Null();

        const string NOT_A_FUNCTION = "No es una función: {0}";
        const string TYPE_MISMATCH = "Discrepancia de tipos: {0} {1} {2}";
        const string UNKNOWN_PREFIX_OPERATOR = "Operador desconocido: {0}{1}";
        const string UNKNOWN_INFIX_OPERATOR = "Operador desconocido: {0} {1} {2}";
        const string UNKNOWN_IDENTIFIER = "Identificador no encontrado: {0}";

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
                return NewError(UNKNOWN_PREFIX_OPERATOR, new List<object> { "-", right.Type.ToString() });
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
                return NewError(UNKNOWN_PREFIX_OPERATOR, new List<object> { @operator, right.Type.ToString() });
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
            else
            {
                return NewError(UNKNOWN_INFIX_OPERATOR, new List<object> { left.Type.ToString(), @operator, right.Type.ToString() });
            }
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
            else if (left.Type != right.Type)
            {
                return NewError(TYPE_MISMATCH, new List<object> { left.Type, @operator, right.Type });
            }
            else
            {
                return NewError(UNKNOWN_INFIX_OPERATOR, new List<object> { left.Type, @operator, right.Type });
            }

        }

        public Object Evaluate(ASTNode node, Environment env)
        {
            Type nodeType = node.GetType();

            if (nodeType == typeof(Program))
            {
                Program program = (Program)node;
                return EvaluateProgram(program, env);
            }
            else if (nodeType == typeof(ExpressionStatement))
            {
                ExpressionStatement expressionStatement = (ExpressionStatement)node;
                Debug.Assert(expressionStatement.Expression != null);
                return Evaluate(expressionStatement.Expression, env);
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
                Object right = Evaluate(prefixExpression.Right, env);
                Debug.Assert(right != null);
                return EvaluatePrefixExpression(prefixExpression.Operator, right);
            }
            else if (nodeType == typeof(Infix))
            {
                Infix infixExpression = (Infix)node;
                Debug.Assert(infixExpression.Left != null && infixExpression.Right != null);
                var left = Evaluate(infixExpression.Left, env);
                var right = Evaluate(infixExpression.Right, env);
                Debug.Assert(left != null && right != null);
                return EvaluateInfixExpression(infixExpression.Operator, left, right);
            }
            else if (nodeType == typeof(Block))
            {
                Block block = (Block)node;
                return EvaluateBlockStatement(block, env);
            }
            else if (nodeType == typeof(If))
            {
                If ifExpression = (If)node;
                return EvaluateIfExpression(ifExpression, env);
            }
            else if (nodeType == typeof(ReturnStatement))
            {
                ReturnStatement returnStatement = (ReturnStatement)node;
                Debug.Assert(returnStatement.ReturnValue != null);
                Object value = Evaluate(returnStatement.ReturnValue, env);

                Debug.Assert(value != null);
                return new Return(value);
            }
            else if (nodeType == typeof(LetStatement))
            {
                LetStatement letStatement = (LetStatement)node;
                Debug.Assert(letStatement.Value != null);
                Object value = Evaluate(letStatement.Value, env);
                Debug.Assert(letStatement.Name != null);
                env.Set(letStatement.Name.Value, value);
            }
            else if (nodeType == typeof(Identifier))
            {
                Identifier identifier = (Identifier)node;
                return EvaluateIdentifier(identifier, env);
            }
            else if (nodeType == typeof(Function))
            {
                Function function = (Function)node;
                Debug.Assert(function.Body != null);
                return new Fnction(function.Parameters, function.Body, env);
            }
            else if (nodeType == typeof(Call))
            {
                Call call = (Call)node;
                Debug.Assert(call.Function != null);    
                Object fn = Evaluate(call.Function, env);

                Debug.Assert(call.Arguments != null);
                List<Object> args = EvaluateExpressions(call.Arguments, env);

                Debug.Assert(fn != null);
                return ApplyFunction(fn, args); 

            }
            else if (nodeType == typeof(StringLiteral))
            {
                StringLiteral stringLiteral = (StringLiteral)node;
                return new String(stringLiteral.Value);

            }

            return null;
        }
        public Object EvaluateProgram(Program program, Environment env)
        {
            Object result = null;

            foreach (var statement in program.Statements)
            {
                result = Evaluate(statement, env);

                if (result != null)
                {
                    Type resultType = result.GetType();

                    if (resultType == typeof(Return))
                    {
                        return ((Return)result).Value;
                    }
                    else if (resultType == typeof(Error))
                    {
                        return result;
                    }
                }
            }

            return result;
        }

        public Object EvaluateBlockStatement(Block block, Environment env)
        {
            Object result = null;

            foreach (var statement in block.Statements)
            {
                result = Evaluate(statement, env);

                if (result != null && (result.Type == ObjectType.RETURN || result.Type == ObjectType.ERROR))
                {
                    return result;
                }
            }

            return result;
        }



        public Object EvaluateIdentifier(Identifier node, Environment env)
        {
            try
            {
                return env[node.Value];
            }
            catch (KeyNotFoundException)
            {
                return NewError(UNKNOWN_IDENTIFIER, new List<object> { node.Value });
            }

        }

        private Error NewError(string message, List<object> args)
        {
            return new Error(string.Format(message, args.ToArray()));
        }


        

        public bool IsTruthy(Object obj)
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
        public Object EvaluateIfExpression(If ifExpression, Environment env)
        {
            Debug.Assert(ifExpression.Condition != null);

            Object condition = Evaluate(ifExpression.Condition, env);
            Debug.Assert(condition != null);

            if (IsTruthy(condition))
            {
                Debug.Assert(ifExpression.Consequence != null);
                return Evaluate(ifExpression.Consequence, env);
            }
            else if (ifExpression.Alternative != null)
            {
                return Evaluate(ifExpression.Alternative, env);
            }
            return NULL;
        }

        public Object ApplyFunction(Object fn, List<Object> args)
        {
            if (fn.Type != ObjectType.FUNCTION)
            {
                return NewError(NOT_A_FUNCTION, new List<object> { fn.Type.ToString() });
            }
            else{
                Fnction function = (Fnction)fn;
                Environment extendedEnv = ExtendFunctionEnvironment(function, args);
                Object evaluated = Evaluate(function.Body, extendedEnv);
                Debug.Assert(evaluated != null);
                return UnwrapReturnValue(evaluated);
            }
        }

        public Environment ExtendFunctionEnvironment(Fnction fn, List<Object> args)
        {
            Environment env = new Environment(outer: fn.Env);

            for (int idx = 0; idx < fn.Parameters.Count; idx++)
            {
                env[fn.Parameters[idx].Value] = args[idx];
            }

            return env;
        }
        public Object UnwrapReturnValue(Object obj)
        {
            if (obj.Type == ObjectType.RETURN)
            {
                return ((Return)obj).Value;
            }
            else
            {
                return obj;
            }
        }

        public List<Object> EvaluateExpressions(List<Expression> expressions, Environment env)
        {
            List<Object> result = new List<Object>();

            foreach (var expression in expressions)
            {
                Object evaluated = Evaluate(expression, env);
                Debug.Assert(evaluated != null);
                result.Add(evaluated);
            }

            return result;
        }

        public Object EvaluateStringInfixExpression(string @operator, Object left, Object right)
        {
            string leftValue = ((String)left).Value;
            string rightValue = ((String)right).Value;

            if (@operator == "+")
            {
                return new String(leftValue + rightValue);
            }
            else if (@operator == "==")
            {
                return ToBooleanObject(leftValue == rightValue);
            }
            else if (@operator == "!=")
            {
                return ToBooleanObject(leftValue != rightValue);
            }
            else
            {
                return NewError(UNKNOWN_INFIX_OPERATOR, new List<object> { left.Type.ToString(), @operator, right.Type.ToString() });
            }
        }




    }


}