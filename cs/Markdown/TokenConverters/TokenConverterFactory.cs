using Markdown.Interfaces;
using Markdown.Tokens;

namespace Markdown.TokenConverters;

public static class TokenConverterFactory
{
    public static ITokenConverter GetConverter(TokenType type)
    {
        return type switch
        {
            TokenType.Text => new TextConverter(),
            TokenType.Emphasis => new EmphasisConverter(),
            TokenType.Strong => new StrongConverter(),
            TokenType.Header => new HeaderConverter(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}