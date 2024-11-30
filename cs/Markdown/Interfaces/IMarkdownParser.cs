using Markdown.Tokens;

namespace Markdown.Interfaces;

public interface IMarkdownParser
{
    IEnumerable<Token> ParseTokens(string markdownText);
}