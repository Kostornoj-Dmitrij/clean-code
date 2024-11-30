using System.Text;
using Markdown.Interfaces;
using Markdown.Renderers;
using Markdown.Tokens;

namespace Markdown.TokenConverters;

public class TextConverter : ITokenConverter
{
    public void Render(Token token, StringBuilder result)
    {
        result.Append(token.Content);
    }
}
