using Markdown.Tokens;

namespace Markdown.Parsers;

public abstract class MarkdownParser
{
    public static IEnumerable<Token> ParseTokens(string markdownText)
    {
        if (markdownText == null)
            throw new ArgumentNullException(nameof(markdownText));

        var context = new MarkdownParseContext
        {
            MarkdownText = markdownText
        };

        while (context.CurrentIndex < context.MarkdownText.Length)
        {
            var current = context.MarkdownText[context.CurrentIndex];
            var next = context.CurrentIndex + 1 < context.MarkdownText.Length ? 
                context.MarkdownText[context.CurrentIndex + 1] : '\0';

            switch (current)
            {
                case '\\':
                    HandleEscapeCharacter(next, context);
                    break;
                case '_':
                    if (next == '_')
                        HandleStrongToken(context);
                    else
                        HandleEmphasisToken(context);
                    break;
                case '#' when (context.CurrentIndex == 0 || 
                               context.MarkdownText[context.CurrentIndex - 1] == '\n') && next == ' ':
                    context.HeaderLevel = HandleHeaderToken(context);
                    break;
                case '\n' when context.Stack.Count > 0 && context.Stack.Peek().Type == TokenType.Header:
                    HandleNewLine(context);
                    break; 
                default:
                    context.Buffer.Append(current);
                    context.CurrentIndex++;
                    break;
            }

            if (context.CurrentIndex != context.MarkdownText.Length || 
                context.Stack.Count <= 0 || context.Stack.Peek().Type != TokenType.Header) continue;
            AddToken(context, TokenType.Text);
            context.Tokens.Add(context.Stack.Pop());
        }
        AddToken(context, TokenType.Text);
        return context.Tokens;
    }

    private static void HandleEscapeCharacter(char next, MarkdownParseContext context)
    {
        if (next is '_' or '#' or '\\')
        {
            if (next != '\\')
                context.Buffer.Append(next);
            context.CurrentIndex += 2;
        }
        else
        {
            context.Buffer.Append('\\');
            context.CurrentIndex++;
        }
    }

    private static void HandleStrongToken(MarkdownParseContext context)
    {
        if (IsValidBoundary(context,"__"))
        {
            HandleTokenBoundary(context, TokenType.Strong);
            context.CurrentIndex += 2;
        }
        else
        {
            context.Buffer.Append("__");
            context.CurrentIndex += 2;
        }
    }

    private static void HandleEmphasisToken(MarkdownParseContext context)
    {
        if (IsValidBoundary(context, "_"))
        {
            HandleTokenBoundary(context, TokenType.Emphasis);
            context.CurrentIndex++;
        }
        else
        {
            context.Buffer.Append('_');
            context.CurrentIndex++;
        }
    }

    private static int HandleHeaderToken(MarkdownParseContext context)
    {
        while (context.CurrentIndex < context.MarkdownText.Length && 
               context.MarkdownText[context.CurrentIndex] == '#')
        {
            context.HeaderLevel++;
            context.CurrentIndex++;
        }

        if (context.CurrentIndex < context.MarkdownText.Length && 
            context.MarkdownText[context.CurrentIndex] == ' ')
        {
            context.CurrentIndex++;

            AddToken(context, TokenType.Text);
            var headerToken = new Token(TokenType.Header)
            {
                HeaderLevel = context.HeaderLevel
            };

            context.Tokens.Add(headerToken);

            var headerEnd = context.MarkdownText.IndexOf('\n', context.CurrentIndex);
            if (headerEnd == -1)
                headerEnd = context.MarkdownText.Length;

            var headerContent = ParseTokens(context.MarkdownText[context.CurrentIndex..headerEnd]);

            foreach (var childToken in headerContent)
            {
                headerToken.Children.Add(childToken);
            }
            context.CurrentIndex = headerEnd;
        }
        else
        {
            context.Buffer.Append('#', context.HeaderLevel);
        }

        return context.HeaderLevel;
    }

    private static void HandleNewLine(MarkdownParseContext context)
    {
        AddToken(context, TokenType.Text);
        context.Tokens.Add(context.Stack.Pop());
        context.CurrentIndex++;
    }

    private static void HandleTokenBoundary(MarkdownParseContext context, TokenType type)
    {
        AddToken(context, TokenType.Text);

        if (context.Stack.Count > 0 && context.Stack.Peek().Type == type)
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
            var newToken = new Token(type);
            context.Stack.Push(newToken);
        }
    }

    private static void AddToken(MarkdownParseContext context, TokenType type)
    {
        if (context.Buffer.Length == 0) return;
        var token = new Token(type, context.Buffer.ToString());
        context.Buffer.Clear();

        if (context.Stack.Count > 0)
            context.Stack.Peek().Children.Add(token);
        else
            context.Tokens.Add(token);
    }

    private static bool IsValidBoundary(MarkdownParseContext context, string delimiter)
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
                   !char.IsLetterOrDigit(context.MarkdownText[index + 1]);
        }

        var closingIndex = text.IndexOf(delimiter, index + delimiter.Length, StringComparison.Ordinal);
        if (closingIndex == -1)
            return false;

        var isInsideWord = (index > 0 && char.IsLetterOrDigit(text[index - 1])) || 
                           (closingIndex + delimiter.Length < text.Length && 
                            char.IsLetterOrDigit(text[closingIndex + delimiter.Length]));
        if (isInsideWord)
            return false;

        if (closingIndex - index <= delimiter.Length)
            return false;
        return index + 1 != closingIndex;
    }
}