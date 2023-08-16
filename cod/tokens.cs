namespace cod
{
public enum TokenType
{
    AND,
    END,
    LITERAL,
    INTEGER,
    PLUS,
    MINUS,
    MULTIPLY,
    DIVIDE,
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
    OR,
    MULTIPLICATION,
    NEGATION,
    NOT_EQ,
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
    private Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
    {
        { "mentira", TokenType.FALSE },
        { "operacion", TokenType.FUNCTION },
        { "atras", TokenType.RETURN },
        { "si", TokenType.IF },
        { "tonces", TokenType.ELSE },
        { "incognita", TokenType.LET },
        { "verdad", TokenType.TRUE }
    };

    public TokenType LookupTokenType(string literal)
    {
        if (keywords.TryGetValue(literal, out TokenType tokenType))
        {
            return tokenType;
        }
        else
        {
            return TokenType.IDENT;
        }
    }


    public override string ToString()
    {
        return $"Token({Type}, {Value})";
    }
    
}
}