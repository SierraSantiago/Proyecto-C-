using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace cod
{

    public enum Precedence
    {
        LOWEST = 1,
        EQUALS = 2,
        LESSGREATER = 3,
        SUM = 4,
        PRODUCT = 5,
        PREFIX = 6,
        CALL = 7
    }


    public class Parser
    {
        private Lexer lexer;
        private Token currentToken;
        private Token peekToken;
        private List<string> errors = new List<string>();

        private Dictionary<TokenType, Func<Expression>> prefixParseFns;
        private Dictionary<TokenType, Func<Expression, Expression>> infixParseFns;

        private readonly Dictionary<TokenType, Precedence> precedences = new Dictionary<TokenType, Precedence>
        {
            {TokenType.EQ, Precedence.EQUALS},
            {TokenType.NOT_EQ, Precedence.EQUALS},
            {TokenType.LT, Precedence.LESSGREATER},
            {TokenType.GT, Precedence.LESSGREATER},
            {TokenType.PLUS, Precedence.SUM},
            {TokenType.MINUS, Precedence.SUM},
            {TokenType.DIVISION, Precedence.PRODUCT},
            {TokenType.MULTIPLY, Precedence.PRODUCT},
            {TokenType.LPAREN, Precedence.CALL}
        };


        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
            currentToken = null;
            peekToken = null;
            errors = new List<string>();

            prefixParseFns = RegisterPrefixFns();
            infixParseFns = RegisterInfixFns();

            NextToken();
            NextToken();

        }


        public List<string> Errors => errors;

        public Program ParseProgram()
        {
            Program program = new Program(new List<Statement>());

            Debug.Assert(currentToken != null);

            while (currentToken.Type != TokenType.END)
            {
                Statement statement = ParseStatement();
                if (statement != null)
                {
                    program.Statements.Add(statement);
                }

                NextToken();
            }

            return program;
        }


        private void NextToken()
        {
            currentToken = peekToken;
            peekToken = lexer.GetNextToken();
        }

        private bool ExpectedToken(TokenType tokenType)
        {
            if (peekToken != null)
            {
                if (peekToken.Type == tokenType)
                {
                    NextToken();
                    return true;
                }
                ExpectedTokenError(tokenType);
            }
            return false;
        }
        private void ExpectedTokenError(TokenType tokenType)
        {
            if (peekToken != null)
            {
                string error = $"Se esperaba que el siguiente token fuera {tokenType} " +
                               $"pero se obtuvo {peekToken.Type}";
                errors.Add(error);
            }
        }

        private bool Match(TokenType expectedType)
        {
            return currentToken.Type == expectedType;
        }



        private Precedence CurrentPrecedence()
        {
            if (precedences.ContainsKey(currentToken.Type))
            {
                return precedences[currentToken.Type];
            }
            return Precedence.LOWEST;
        }
        private Block ParseBlock()
        {
            if (currentToken != null)
            {
                Block blockStatement = new Block(token: currentToken, statements: new List<Statement>());

                NextToken();

                while (currentToken.Type != TokenType.RBRACE
                        && currentToken.Type != TokenType.END)
                {
                    Statement statement = ParseStatement();

                    if (statement != null)
                    {
                        blockStatement.Statements.Add(statement);
                    }

                    NextToken();
                }

                return blockStatement;
            }
            return null; // Devuelve null en caso de que _current_token sea nulo.
        }
        private Boolea ParseBoolean()
        {
            if (currentToken != null)
            {
                return new Boolea(currentToken, currentToken.Type == TokenType.TRUE);
            }
            return null;
        }
        private Call ParseCall(Expression function)
        {
            if (currentToken != null)
            {
                Call call = new Call(currentToken, function);
                call.Arguments = ParseCallArguments();

                return call;
            }
            return null; // Devuelve null en caso de que currentToken sea nulo.
        }
        private List<Expression> ParseCallArguments()
        {
            List<Expression> arguments = new List<Expression>();

            if (peekToken != null && peekToken.Type == TokenType.RPAREN)
            {
                NextToken(); // Avanzar al siguiente token
                return arguments;
            }

            NextToken(); // Avanzar al siguiente token

            // Parsear la primera expresión y agregarla a la lista de argumentos
            Expression expression = ParseExpression(Precedence.LOWEST);
            if (expression != null)
            {
                arguments.Add(expression);
            }

            while (peekToken != null && peekToken.Type == TokenType.COMMA)
            {
                NextToken(); // Avanzar al siguiente token (',')

                NextToken(); // Avanzar al siguiente token después de la coma

                // Parsear la siguiente expresión y agregarla a la lista de argumentos
                expression = ParseExpression(Precedence.LOWEST);
                if (expression != null)
                {
                    arguments.Add(expression);
                }
            }

            if (!ExpectedToken(TokenType.RPAREN))
            {
                return null;
            }

            return arguments;
        }
        private Expression ParseExpression(Precedence precedence)
        {
            Debug.Assert(currentToken != null);

            try
            {
                Func<Expression> prefixParseFn = prefixParseFns[currentToken.Type];
                Expression leftExpression = prefixParseFn();

                Debug.Assert(peekToken != null);
                while (peekToken.Type != TokenType.SEMICOLON && precedence < PeekPrecedence())
                {
                    try
                    {
                        Func<Expression, Expression> infixParseFn = infixParseFns[peekToken.Type];
                        NextToken();

                        Debug.Assert(leftExpression != null);
                        leftExpression = infixParseFn(leftExpression);
                    }
                    catch (KeyNotFoundException)
                    {
                        return leftExpression;
                    }
                }

                return leftExpression;
            }
            catch (KeyNotFoundException)
            {
                string message = $"No se encontró ninguna función para analizar {currentToken.Value}";
                errors.Add(message);
                return null;
            }
        }

        private ExpressionStatement ParseExpressionStatement()
        {
            Debug.Assert(currentToken != null);
            ExpressionStatement expressionStatement = new ExpressionStatement(currentToken);

            expressionStatement.Expression = ParseExpression(Precedence.LOWEST);

            Debug.Assert(peekToken != null);
            if (peekToken.Type == TokenType.SEMICOLON)
            {
                NextToken();
            }

            return expressionStatement;
        }

        private Expression ParseGroupedExpression()
        {
            NextToken();

            Expression expression = ParseExpression(Precedence.LOWEST);

            if (!ExpectedToken(TokenType.RPAREN))
            {
                return null;
            }

            return expression;
        }
        private Function ParseFunction()
        {
            if (currentToken != null)
            {
                Function function = new Function(currentToken);

                if (!ExpectedToken(TokenType.LPAREN))
                {
                    return null;
                }

                function.Parameters = ParseFunctionParameters();

                if (!ExpectedToken(TokenType.LBRACE))
                {
                    return null;
                }

                function.Body = ParseBlock();

                return function;
            }

            return null;
        }
        private List<Identifier> ParseFunctionParameters()
        {
            List<Identifier> parameters = new List<Identifier>();

            if (peekToken != null && peekToken.Type == TokenType.RPAREN)
            {
                NextToken();

                return parameters;
            }

            NextToken();

            if (currentToken != null)
            {
                Identifier identifier = new Identifier(currentToken, currentToken.Value);
                parameters.Add(identifier);
            }

            while (peekToken != null && peekToken.Type == TokenType.COMMA)
            {
                NextToken();
                NextToken();


                if (currentToken != null)
                {
                    Identifier identifier = new Identifier(currentToken, currentToken.Value);
                    parameters.Add(identifier);
                }
            }

            if (!ExpectedToken(TokenType.RPAREN))
            {
                return new List<Identifier>();
            }

            return parameters;
        }
        private Identifier ParseIdentifier()
        {
            if (currentToken != null)
            {
                return new Identifier(currentToken, currentToken.Value);
            }
            return null;
        }
        private If ParseIf()
        {
            if (currentToken != null)
            {
                If ifExpression = new If(currentToken);

                if (!ExpectedToken(TokenType.LPAREN))
                {
                    return null;
                }

                NextToken();

                ifExpression.Condition = ParseExpression(Precedence.LOWEST);

                if (!ExpectedToken(TokenType.RPAREN))
                {
                    return null;
                }

                if (!ExpectedToken(TokenType.LBRACE))
                {
                    return null;
                }

                ifExpression.Consequence = ParseBlock();

                if (peekToken != null && peekToken.Type == TokenType.ELSE)
                {
                    NextToken();

                    if (!ExpectedToken(TokenType.LBRACE))
                    {
                        return null;
                    }

                    ifExpression.Alternative = ParseBlock();
                }

                return ifExpression;
            }
            return null;
        }
        private Infix ParseInfixExpression(Expression left)
        {
            if (currentToken != null)
            {
                Token token = currentToken;
                string op = token.Value;
                Precedence precedence = CurrentPrecedence();

                NextToken();

                Expression right = ParseExpression(precedence);

                return new Infix(token, left, op, right);
            }
            return null;
        }

        private Integer ParseInteger()
        {
            if (currentToken != null)
            {
                Token token = currentToken;
                Integer integer = new Integer(token);

                try
                {
                    integer.Value = int.Parse(token.Value);
                }
                catch (FormatException)
                {
                    string message = $"No se pudo analizar '{token.Value}' como un entero.";
                    errors.Add(message);
                    return null;
                }

                NextToken();
                return integer;
            }
            return null;
        }
        private LetStatement ParseLetStatement()
        {
            if (currentToken != null)
            {
                Token token = currentToken;
                LetStatement letStatement = new LetStatement(token);

                if (!ExpectedToken(TokenType.IDENT))
                {
                    return null;
                }

                letStatement.Name = ParseIdentifier();

                if (!ExpectedToken(TokenType.ASSIGN))
                {
                    return null;
                }

                NextToken();

                letStatement.Value = ParseExpression(Precedence.LOWEST);

                if (peekToken != null && peekToken.Type == TokenType.SEMICOLON)
                {
                    NextToken();
                }

                return letStatement;
            }
            return null;
        }
        private ParsePrefixExpression ParsePrefixExpression()
        {
            if (currentToken != null)
            {
                Token token = currentToken;
                ParsePrefixExpression prefixExpression = new ParsePrefixExpression(token, currentToken.Value);

                NextToken();

                prefixExpression.Right = ParseExpression(Precedence.PREFIX);
                return prefixExpression;
            }
            return null;
        }
        private ReturnStatement ParseReturnStatement()
        {
            if (currentToken != null)
            {
                Token token = currentToken;
                ReturnStatement returnStatement = new ReturnStatement(token);

                NextToken();

                returnStatement.ReturnValue = ParseExpression(Precedence.LOWEST);

                if (Match(TokenType.SEMICOLON))
                {
                    NextToken();
                }

                return returnStatement;
            }
            return null;
        }
        private Statement ParseStatement()
        {
            if (currentToken != null)
            {
                if (currentToken.Type == TokenType.LET)
                {
                    return ParseLetStatement();
                }
                else if (currentToken.Type == TokenType.RETURN)
                {
                    return ParseReturnStatement();
                }
                else
                {
                    return ParseExpressionStatement();
                }
            }
            return null;
        }
        private Expression ParseStringLiteral()
        {
            if (currentToken != null)
            {
                return new StringLiteral(currentToken, currentToken.Value);
            }
            return null;
        }

        private Precedence PeekPrecedence()
        {
            if (peekToken != null)
            {
                if (precedences.ContainsKey(peekToken.Type))
                {
                    return precedences[peekToken.Type];
                }
            }
            return Precedence.LOWEST;
        }
        private Dictionary<TokenType, Func<Expression, Expression>> RegisterInfixFns()
        {
            return new Dictionary<TokenType, Func<Expression, Expression>>
    {
        { TokenType.PLUS, ParseInfixExpression },
        { TokenType.MINUS, ParseInfixExpression },
        { TokenType.DIVISION, ParseInfixExpression },
        { TokenType.MULTIPLICATION, ParseInfixExpression },
        { TokenType.EQ, ParseInfixExpression },
        { TokenType.NOT_EQ, ParseInfixExpression },
        { TokenType.LT, ParseInfixExpression },
        { TokenType.GT, ParseInfixExpression },
        { TokenType.LPAREN, ParseCall },
    };
        }

        private Dictionary<TokenType, Func<Expression>> RegisterPrefixFns()
        {
            return new Dictionary<TokenType, Func<Expression>>
    {
        { TokenType.FALSE, ParseBoolean },
        { TokenType.FUNCTION, ParseFunction },
        { TokenType.IDENT, ParseIdentifier },
        { TokenType.IF, ParseIf },
        { TokenType.INTEGER, ParseInteger },
        { TokenType.LPAREN, ParseGroupedExpression },
        { TokenType.MINUS, ParsePrefixExpression },
        { TokenType.NEGATION, ParsePrefixExpression },
        { TokenType.TRUE, ParseBoolean },
    };
        }

    }
}