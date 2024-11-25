namespace Markdown.Tokens;

public class Token
{
    public TokenType Type { get; }
    public string Content { get; set; }
    public List<Token> Children { get; }
    public int HeaderLevel { get; init; }
    public Token(TokenType type, string content, List<Token>? children = null)
    {
        Type = type;
        Content = content;
        Children = children ?? [];
        HeaderLevel = 1;
    }

    public Token(TokenType type)
    {
        Type = type;
        Content = string.Empty;
        Children = [];
    }
}