namespace Markdown.Tokens;

public class Token
{
    public TokenType Type { get; }
    public string Content { get; set; }
    public List<Token> Children { get; init; }
    public int HeaderLevel { get; init; }

    public Token(TokenType type, string content, List<Token>? children = null)
    {
        Type = type;
        Content = content;
        Children = children ?? [];
        HeaderLevel = 1;
    }

    public Token(TokenType type) : this(type, string.Empty) { }
    public Token(TokenType type, List<Token>? children = null)
        : this(type, string.Empty, children) { }
}