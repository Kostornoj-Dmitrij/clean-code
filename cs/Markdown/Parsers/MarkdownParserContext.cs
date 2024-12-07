using System.Text;
using Markdown.Interfaces;
using Markdown.Tokens;

namespace Markdown.Parsers;

public class MarkdownParseContext
{
    public Stack<Token> Stack { get; } = new();
    public StringBuilder Buffer { get; } = new();
    public List<Token> Tokens { get; } = [];
    public List<int> IntersectedIndexes { get; } = [];
    public string MarkdownText { get; init; } = "";
    public int CurrentIndex { get; set; }
    public int HeaderLevel { get; set; }
    public required IMarkdownParser Parser { get; init; }
}