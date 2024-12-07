using System.Text;
using Markdown.Tokens;

namespace Markdown.TokenConverters;

public class EmphasisConverter : TokenConverterBase
{
    public override void Render(Token token, StringBuilder result)
    {
        result.Append("<em>");
        RenderChildren(token, result);
        result.Append("</em>");
    }
}