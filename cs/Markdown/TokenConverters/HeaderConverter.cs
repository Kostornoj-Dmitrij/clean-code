using System.Text;
using Markdown.Interfaces;
using Markdown.Renderers;
using Markdown.Tokens;

namespace Markdown.TokenConverters;

public class HeaderConverter : TokenConverterBase
{
    public override void Render(Token token, StringBuilder result)
    {
        var level = token.HeaderLevel;
        result.Append($"<h{level}>");
        RenderChildren(token, result);
        result.Append($"</h{level}>");
    }
}
