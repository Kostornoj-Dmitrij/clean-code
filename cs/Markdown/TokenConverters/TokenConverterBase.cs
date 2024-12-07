using System.Text;
using Markdown.Interfaces;
using Markdown.Tokens;

namespace Markdown.TokenConverters;

public abstract class TokenConverterBase : ITokenConverter
{
    public abstract void Render(Token token, StringBuilder result);

    protected static void RenderChildren(Token token, StringBuilder result)
    {
        foreach (var child in token.Children)
        {
            var converter = TokenConverterFactory.GetConverter(child.Type);
            converter.Render(child, result);
        }
    }
}