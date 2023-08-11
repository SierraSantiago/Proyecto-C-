using System;

public class Lexer
{
    private readonly string _input;
    private int _position;

    public Lexer(string input)
    {
        _input = input;
        _position = 0;
    }

    private char CurrentChar()
    {
        if (_position < _input.Length)
            return _input[_position];
        else
            return '\0'; // End of input
    }

    private void Advance()
    {
        _position++;
    }

    private void SkipWhitespace()
    {
        while (char.IsWhiteSpace(CurrentChar()))
        {
            Advance();
        }
    }

    private string CollectInteger()
    {
        string result = "";
        while (char.IsDigit(CurrentChar()))
        {
            result += CurrentChar();
            Advance();
        }
        return result;
    }

    public Token GetNextToken()
    {
        while (CurrentChar() != '\0')
        {
            if (char.IsWhiteSpace(CurrentChar()))
            {
                SkipWhitespace();
                continue;
            }

            if (char.IsDigit(CurrentChar()))
            {
                return new Token(TokenType.Integer, CollectInteger());
            }

            if (CurrentChar() == '+')
            {
                Advance();
                return new Token(TokenType.Plus, "+");
            }

            if (CurrentChar() == '-')
            {
                Advance();
                return new Token(TokenType.Minus, "-");
            }

            if (CurrentChar() == '*')
            {
                Advance();
                return new Token(TokenType.Multiply, "*");
            }

            if (CurrentChar() == '=')
            {
                Advance();
                return new Token(TokenType.EQ, "=");
            }

            if (CurrentChar() == '(')
            {
                Advance();
                return new Token(TokenType.LPAREN, "(");
            }
            if (CurrentChar() == ')')
            {
                Advance();
                return new Token(TokenType.RPAREN, ")");
            }
            if (CurrentChar() == '{')
            {
                Advance();
                return new Token(TokenType.LBRACE, "{");
            }
            if (CurrentChar() == '}')
            {
                Advance();
                return new Token(TokenType.RBRACE, "}");
            }
            if (CurrentChar() == ',')
            {
                Advance();
                return new Token(TokenType.COMMA, ",");
            }
            if (CurrentChar() == ';')
            {
                Advance();
                return new Token(TokenType.SEMICOLON, ";");
            }
            if (CurrentChar() == '<')
            {
                Advance();
                return new Token(TokenType.LT, "<");
            }
            if (CurrentChar() == '>')
            {
                Advance();
                return new Token(TokenType.GT, ">");
            }

            if (CurrentChar() == '!')
            {
                Advance();
                return new Token(TokenType.Multiply, "*");
            }

            {
                Advance();
                return new Token(TokenType.Divide, "/");
            }

            throw new Exception($"Error: Caracter inesperado '{CurrentChar()}'");
        }

        return new Token(TokenType.EOF, "\0");
    }
}