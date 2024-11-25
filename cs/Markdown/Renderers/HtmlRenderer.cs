using System.Text;
using Markdown.Tokens;

namespace Markdown.Renderers;

public class HtmlRenderer
{
    public string Render(IEnumerable<Token> tokens)
    {
        var result = new StringBuilder();
        foreach (var token in tokens)
        {
            RenderToken(token, result);
        }
        return result.ToString();
    }

    private void RenderToken(Token token, StringBuilder result)
    {
        switch (token.Type)
        {
            case TokenType.Text:
                result.Append(token.Content);
                break;
            case TokenType.Emphasis:
                result.Append("<em>");
                RenderChildren(token, result);
                result.Append("</em>");
                break;
            case TokenType.Strong:
                result.Append("<strong>");
                RenderChildren(token, result);
                result.Append("</strong>");
                break;
            case TokenType.Header:
                var level = token.HeaderLevel;
                result.Append($"<h{level}>");
                RenderChildren(token, result);
                result.Append($"</h{level}>");
                break;
            default:
                result.Append(token.Content);
                break;
        }
    }

    private void RenderChildren(Token token, StringBuilder result)
    {
        if (token.Children.Count > 0)
        {
            foreach (var child in token.Children)
            {
                RenderToken(child, result);
            }
        }
        else
            result.Append(token.Content);
    }
}