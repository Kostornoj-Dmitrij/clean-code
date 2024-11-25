using Markdown.Parsers;
using Markdown.Renderers;

namespace Markdown;

public class Md
{
    private readonly HtmlRenderer renderer;

    public Md()
    {
        renderer = new HtmlRenderer();
    }

    public string Render(string markdownText)
    {
        var tokens = MarkdownParser.ParseTokens(markdownText);
        return renderer.Render(tokens);
    }
}