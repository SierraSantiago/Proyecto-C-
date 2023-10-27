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
        GTE,
        SPACE,
        WHILE,
        DO,
        BREAK,
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
        public Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
    {
        { "mentira", TokenType.FALSE },
        { "operacion", TokenType.FUNCTION },
        { "devuelve", TokenType.RETURN },
        { "si", TokenType.IF },
        { "tonces", TokenType.ELSE },
        { "incognita", TokenType.LET },
        { "verdad", TokenType.TRUE },
        { "mientras", TokenType.WHILE },
        { "haz", TokenType.DO },
        { "termina", TokenType.BREAK },
    };
    
    public bool ContainsKeyword(string keyword)
    {
        return keywords.ContainsKey(keyword);
    }
    
    

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

        public string[] SplitString(string input)
        {
            // Dividir el string usando espacios como delimitador
            string[] words = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return words;
        }


        public override string ToString()
        {
            return $"Token({Type}, {Value})";
        }

    }
}