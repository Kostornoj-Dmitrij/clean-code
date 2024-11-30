using Markdown.Interfaces;
using Markdown.Parsers;
using Markdown.Tokens;

namespace Markdown.TokenHandlers;

public abstract class BoundaryTokenHandler : ITokenHandler
{
    protected abstract string Delimiter { get; }
    protected abstract TokenType TokenType { get; }

    public abstract bool CanHandle(char current, char next, MarkdownParseContext context);

    public void Handle(MarkdownParseContext context)
    {
        if (IsValidBoundary(context))
        {
            HandleTokenBoundary(context);
        }
        else
        {
            context.Buffer.Append(Delimiter);
            context.CurrentIndex += Delimiter.Length;
        }
    }

    private bool IsValidBoundary(MarkdownParseContext context)
    {
        var index = context.CurrentIndex;
        var text = context.MarkdownText;

        if (context.Stack.Count > 0)
        {
            if (context.Buffer.Length == 0) 
                return false;
            if (index == 0 || index == text.Length - 1)
                return true;
            return !char.IsLetterOrDigit(text[index - 1]) || 
                   !char.IsLetterOrDigit(text[index + 1]);
        }

        var closingIndex = text.IndexOf(Delimiter, index + Delimiter.Length, StringComparison.Ordinal);
        if (closingIndex == -1)
            return false;

        var isInsideWord = (index > 0 && char.IsLetterOrDigit(text[index - 1])) || 
                           (closingIndex + Delimiter.Length < text.Length && 
                            char.IsLetterOrDigit(text[closingIndex + Delimiter.Length]));
        if (isInsideWord)
            return false;

        if (closingIndex - index <= Delimiter.Length)
            return false;

        return index + 1 != closingIndex;
    }

    private void HandleTokenBoundary(MarkdownParseContext context)
    {
        MarkdownParser.AddToken(context, TokenType.Text);

        if (context.Stack.Count > 0 && context.Stack.Peek().Type == TokenType)
        {
            var completedToken = context.Stack.Pop();

            completedToken.Content = completedToken.Children.Count > 0 ? string.Empty : completedToken.Content;
            context.Buffer.Clear();

            if (context.Stack.Count > 0)
                context.Stack.Peek().Children.Add(completedToken);
            else
                context.Tokens.Add(completedToken);
        }
        else
        {
            var newToken = new Token(TokenType);
            context.Stack.Push(newToken);
        }

        context.CurrentIndex += Delimiter.Length;
    }
}
