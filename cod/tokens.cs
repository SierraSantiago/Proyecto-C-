public enum TokenType
{
    Integer,
    Plus,
    Minus,
    Multiply,
    Divide,
    EOF,
    ASSIGN,
    COMMA,
    DIVISION,
    ELSE,
    EQ,
    FALSE,
    FUNCTION,
    GT,
    IDENT,
    IF,
    ILLEGAL,
    INT,
    LBRACE,
    LET,
    LPAREN,
    LT,
    MINUS,
    MULTIPLICATION,
    NEGATION,
    NOT_EQ,
    PLUS,
    RETURN,
    RBRACE,
    RPAREN,
    SEMICOLON,
    TRUE,
    LTE,
    GTE    
}

public class Token
{
    public TokenType Type { get; }
    public string Value { get; }

    public Token(TokenType type, string value)
    {
        Type = type;
        Value = value;
    }

    public override string ToString()
    {
        return $"Token({Type}, {Value})";
    }
    
}