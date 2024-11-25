using Markdown.Tokens;
using System.Text;
namespace Markdown.Parsers;

public class MarkdownParseContext
{
    public Stack<Token> Stack { get; set; } = new();
    public List<Token> Tokens { get; set; } = new();
    public StringBuilder Buffer { get; set; } = new();
    public string MarkdownText { get; set; } = "";
    public int CurrentIndex { get; set; }
    public int HeaderLevel { get; set; }
}