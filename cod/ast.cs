using System;
using System.Collections.Generic;

namespace cod
{
    public abstract class ASTNode
    {
        public abstract string TokenLiteral();
        public abstract override string ToString();
    }

    public class Statement : ASTNode
    {
        public Token Token { get; }

        public Statement(Token token)
        {
            Token = token;
        }

        public override string TokenLiteral()
        {
            return Token.Value;
        }

        public override string ToString()
        {
            return Token.Value;
        }
    }

    public class Expression : ASTNode
    {
        public Token Token { get; }

        public Expression(Token token)
        {
            Token = token;
        }

        public override string TokenLiteral()
        {
            return Token.Value;
        }

        public override string ToString()
        {
            return Token.Value;
        }
    }

    public class Program : ASTNode
    {
        public List<Statement> Statements { get; }

        public Program(List<Statement> statements)
        {
            Statements = statements;
        }

        public override string TokenLiteral()
        {
            if (Statements.Count > 0)
            {
                return Statements[0].TokenLiteral();
            }
            return "";
        }

        public override string ToString()
        {
            List<string> statementStrings = new List<string>();
            foreach (var statement in Statements)
            {
                statementStrings.Add(statement.ToString());
            }
            return string.Join("", statementStrings);
        }
    }

    public class Identifier : Expression
    {
        public string Value { get; }

        public Identifier(Token token, string value)
            : base(token)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }

    public class LetStatement : Statement
    {
        public Identifier Name { get; }
        public Expression Value { get; }

        public LetStatement(Token token, Identifier name = null, Expression value = null)
            : base(token)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return $"{TokenLiteral()} {Name} = {Value};";
        }
    }
    public class ReturnStatement : Statement
    {
        public Expression ReturnValue { get; }

        public ReturnStatement(Token token, Expression returnValue = null)
            : base(token)
        {
            ReturnValue = returnValue;
        }

        public override string ToString()
        {
            return $"{TokenLiteral()} {ReturnValue};";
        }
    }
    public class ExpressionStatement : Statement
    {
        public Expression Expression { get;set; }

        public ExpressionStatement(Token token, Expression expression = null)
            : base(token)
        {
            Expression = expression;
        }

        public override string ToString()
        {
            return Expression.ToString();
        }
    }
    public class Integer : Expression
    {
        public int? Value { get; }

        public Integer(Token token, int? value = null)
            : base(token)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.HasValue ? Value.ToString() : "null";
        }
    }
    public class ParsePrefixExpression : Expression
    {
        public string Operator { get; }
        public Expression Right { get; }

        public ParsePrefixExpression(Token token, string @operator, Expression right = null)
            : base(token)
        {
            Operator = @operator;
            Right = right;
        }

        public override string ToString()
        {
            return $"({Operator}{Right})";
        }
    }
    public class Infix : Expression
    {
        public Expression Left { get; }
        public string Operator { get; }
        public Expression Right { get; }

        public Infix(Token token, Expression left, string @operator, Expression right = null)
            : base(token)
        {
            Left = left;
            Operator = @operator;
            Right = right;
        }

        public override string ToString()
        {
            return $"({Left} {Operator} {Right})";
        }
    }

    public class Boolean : Expression
    {
        public bool? Value { get; }

        public Boolean(Token token, bool? value = null)
            : base(token)
        {
            Value = value;
        }

        public override string ToString()
        {
            return TokenLiteral();
        }
    }

    public class Block : Statement
    {
        public List<Statement> Statements { get; }

        public Block(Token token, List<Statement> statements)
            : base(token)
        {
            Statements = statements;
        }

        public override string ToString()
        {
            var outList = new List<string>();
            foreach (var statement in Statements)
            {
                outList.Add(statement.ToString());
            }

            return string.Join("", outList);
        }
    }
    public class If : Expression
    {
        public Expression Condition { get;set; }
        public Block Consequence { get;set; }
        public Block Alternative { get;set; }

        public If(Token token, Expression condition = null, Block consequence = null, Block alternative = null)
            : base(token)
        {
            Condition = condition;
            Consequence = consequence;
            Alternative = alternative;
        }

        public override string ToString()
        {
            var output = $"si {Condition} {Consequence}";

            if (Alternative != null)
            {
                output += $" si_no {Alternative}";
            }

            return output;
        }
    }
    public class Function : Expression
    {
        public List<Identifier> Parameters { get;set; }
        public Block Body { get;set; }

        public Function(Token token, List<Identifier> parameters = null, Block body = null)
            : base(token)
        {
            Parameters = parameters ?? new List<Identifier>();
            Body = body;
        }

        public override string ToString()
        {
            var paramList = new List<string>();
            foreach (var parameter in Parameters)
            {
                paramList.Add(parameter.ToString());
            }

            var parameters = string.Join(", ", paramList);

            return $"{TokenLiteral()}({parameters}) {Body}";
        }
    }
    public class Call : Expression
    {
        public Expression Function { get; }
        public List<Expression> Arguments { get; set; }

        public Call(Token token, Expression function, List<Expression> arguments = null)
            : base(token)
        {
            Function = function;
            Arguments = arguments ?? new List<Expression>();
        }

        public override string ToString()
        {
            var argList = new List<string>();
            foreach (var argument in Arguments)
            {
                argList.Add(argument.ToString());
            }

            var args = string.Join(", ", argList);

            return $"{Function}({args})";
        }
    }
    public class StringLiteral : Expression
    {
        public string Value { get; }

        public StringLiteral(Token token, string value)
            : base(token)
        {
            Value = value;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

}