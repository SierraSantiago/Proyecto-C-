using cod;

namespace Parser
{
    public class Parser
    {
        private Lexer lexer;
        private Token currentToken;
        private Token peekToken;
        private List<string> errors = new List<string>();

        private Dictionary<TokenType, Func<Expression>> prefixParseFns;
        private Dictionary<TokenType, Func<Expression, Expression>> infixParseFns;
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


            prefixParseFns = new Dictionary<TokenType, Func<Expression>>
            {
                {TokenType.FALSE, ParseBoolean},
                {TokenType.FUNCTION, ParseFunction},
                {TokenType.IDENT, ParseIdentifier},
                {TokenType.IF, ParseIf},
                {TokenType.INT, ParseInteger},
                {TokenType.LPAREN, ParseGroupedExpression},
                {TokenType.MINUS, ParsePrefixExpression},
                {TokenType.NEGATION, ParsePrefixExpression},
                {TokenType.TRUE, ParseBoolean},
                {TokenType.LITERAL, ParseStringLiteral}
            };

            infixParseFns = new Dictionary<TokenType, Func<Expression, Expression>>
            {
                {TokenType.PLUS, ParseInfixExpression},
                {TokenType.MINUS, ParseInfixExpression},
                {TokenType.DIVISION, ParseInfixExpression},
                {TokenType.MULTIPLY, ParseInfixExpression},
                {TokenType.EQ, ParseInfixExpression},
                {TokenType.NOT_EQ, ParseInfixExpression},
                {TokenType.LT, ParseInfixExpression},
                {TokenType.GT, ParseInfixExpression},
                {TokenType.LPAREN, ParseCall}
            };

            NextToken();
            NextToken();
        }

        public List<string> Errors => errors;

        public Program ParseProgram()
        {
            var program = new Program(new List<Statement>());

            while (currentToken.Type != TokenType.EOF)
            {
                var statement = ParseStatement();
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

        private bool Expect(TokenType expectedType)
        {
            if (currentToken.Type == expectedType)
            {
                NextToken();
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool Match(TokenType expectedType)
        {
            return currentToken.Type == expectedType;
        }

        private void ExpectedTokenError(TokenType expectedType)
        {
            errors.Add($"Expected token {expectedType}, but got {currentToken.Type}");
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
                        && currentToken.Type != TokenType.EOF)
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
        private bool ParseBoolean()
        {
            if (currentToken != null)
            {
                return currentToken.Type == TokenType.TRUE;
            }
            return false; // Devuelve false en caso de que currentToken sea nulo.
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

            if (!Expect(TokenType.RPAREN))
            {
                return null;
            }

            return arguments;
        }
        private Expression ParseExpression(Precedence precedence)
        {
            if (currentToken != null)
            {
                try
                {
                    Func<Expression> prefixParseFn = prefixParseFns[currentToken.Type];
                    Expression leftExpression = prefixParseFn();

                    while (peekToken != null && peekToken.Type != TokenType.SEMICOLON &&
                           precedence < PeekPrecedence())
                    {
                        try
                        {
                            Func<Expression, Expression> infixParseFn = infixParseFns[peekToken.Type];
                            NextToken(); // Avanza al siguiente token.

                            if (leftExpression != null)
                            {
                                leftExpression = infixParseFn(leftExpression);
                            }
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
            return null;
        }
        private ExpressionStatement ParseExpressionStatement()
        {
            if (currentToken != null)
            {
                ExpressionStatement expressionStatement = new ExpressionStatement(currentToken);

                expressionStatement.Expression = ParseExpression(Precedence.LOWEST);

                if (peekToken != null && peekToken.Type == TokenType.SEMICOLON)
                {
                    NextToken();
                }

                return expressionStatement;
            }

            return null;
        }
        private Expression ParseGroupedExpression()
        {
            NextToken();

            Expression expression = ParseExpression(Precedence.LOWEST);

            if (!Expect(TokenType.RPAREN))
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

                if (!Expect(TokenType.LPAREN))
                {
                    return null;
                }

                function.Parameters = ParseFunctionParameters();

                if (!Expect(TokenType.LBRACE))
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

            if (!Expect(TokenType.RPAREN))
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

                if (!Expect(TokenType.LPAREN))
                {
                    return null;
                }

                NextToken();

                ifExpression.Condition = ParseExpression(Precedence.LOWEST);

                if (!Expect(TokenType.RPAREN))
                {
                    return null;
                }

                if (!Expect(TokenType.LBRACE))
                {
                    return null;
                }

                ifExpression.Consequence = ParseBlock();

                if (peekToken != null && peekToken.Type == TokenType.ELSE)
                {
                    NextToken();

                    if (!Expect(TokenType.LBRACE))
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

                if (!Expect(TokenType.IDENT))
                {
                    return null;
                }

                letStatement.Name = ParseIdentifier();

                if (!Expect(TokenType.ASSIGN))
                {
                    return null;
                }

                NextToken();

                letStatement.Value = ParseExpression(Precedence.LOWEST);

                if (currentToken != null && currentToken.Type == TokenType.SEMICOLON)
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









        // Resto de los métodos de análisis sintáctico...
    }
}