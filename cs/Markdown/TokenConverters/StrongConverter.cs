using System.Text;
using Markdown.Tokens;

namespace Markdown.TokenConverters;

public class StrongConverter : TokenConverterBase
{
    public override void Render(Token token, StringBuilder result)
    {
        result.Append("<strong>");
        RenderChildren(token, result);
        result.Append("</strong>");
    }
}