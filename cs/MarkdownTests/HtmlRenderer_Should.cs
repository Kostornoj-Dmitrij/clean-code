using FluentAssertions;
using Markdown.Renderers;
using Markdown.Tokens;
using NUnit.Framework;

namespace MarkdownTests;

[TestFixture]
public class HtmlRenderer_Should
{
    private readonly HtmlRenderer renderer = new();

    [Test]
    public void Render_ShouldHandleTextWithoutTags()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.Text, "Текст без тегов.")
        };

        var result = renderer.Render(tokens);

        result.Should().Be("Текст без тегов.");
    }

    [Test]
    public void Render_ShouldHandleEmphasisTags()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.Text, "Это "),
            new Token(TokenType.Emphasis, "курсив") { Children = new List<Token> { new Token(TokenType.Text, "курсив") } },
            new Token(TokenType.Text, " текст")
        };

        var result = renderer.Render(tokens);

        result.Should().Be("Это <em>курсив</em> текст");
    }

    [Test]
    public void Render_ShoulHandleStrongTags()
    {
        var strongToken = new Token(TokenType.Strong, string.Empty);
        strongToken.Children.Add(new Token(TokenType.Text, "полужирный")); // Добавляем текст как дочерний токен

        var tokens = new List<Token>
        {
            new Token(TokenType.Text, "Это "),
            strongToken,
            new Token(TokenType.Text, " текст")
        };

        var result = renderer.Render(tokens);

        result.Should().Be("Это <strong>полужирный</strong> текст");
    }

    [Test]
    public void Render_ShouldHandleHeaderTags()
    {
        var headerToken = new Token(TokenType.Header, string.Empty);
        headerToken.Children.Add(new Token(TokenType.Text, "Заголовок"));

        var tokens = new List<Token> { headerToken };

        var result = renderer.Render(tokens);

        result.Should().Be("<h1>Заголовок</h1>");
    }

    [Test]
    public void Render_ShouldHandleNestedTags()
    {
        var headToken = new Token(TokenType.Header, string.Empty);
        var strongToken = new Token(TokenType.Strong, string.Empty);
        var emphasisToken = new Token(TokenType.Emphasis, string.Empty);

        emphasisToken.Children.Add(new Token(TokenType.Text, "курсивом"));
        strongToken.Children.Add(new Token(TokenType.Text, "полужирным текстом с "));
        strongToken.Children.Add(emphasisToken);
        headToken.Children.Add(new Token(TokenType.Text, "заголовок с "));
        headToken.Children.Add(strongToken);
        var tokens = new List<Token>
        {
            new Token(TokenType.Text, "Это "),
            headToken
        };

        var result = renderer.Render(tokens);

        result.Should().Be("Это <h1>заголовок с <strong>полужирным " +
                           "текстом с <em>курсивом</em></strong></h1>");
    }

    [Test]
    public void Render_ShouldHandleEmptyTags()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.Text, "Это "),
            new Token(TokenType.Emphasis, string.Empty),
            new Token(TokenType.Text, " текст")
        };


        var result = renderer.Render(tokens);

        result.Should().Be("Это <em></em> текст");
    }

    [Test]
    public void Render_ShouldHandleMultipleTags()
    {
        var tokens = new List<Token>
        {
            new Token(TokenType.Text, "Это "),
            new Token(TokenType.Strong, "полужирный") { Children = { new Token(TokenType.Text, "полужирный") } },
            new Token(TokenType.Text, " и "),
            new Token(TokenType.Emphasis, string.Empty) { Children = { new Token(TokenType.Text, "курсив") } },
            new Token(TokenType.Text, " текст.")
        };

        var result = renderer.Render(tokens);

        result.Should().Be("Это <strong>полужирный</strong> и <em>курсив</em> текст.");
    }

    [Test]
    public void Render_ShouldHandleNestedTagsWithMultipleLevels()
    {
        var innerStrongToken = new Token(TokenType.Strong, string.Empty);
        innerStrongToken.Children.Add(new Token(TokenType.Text, "полужирный заголовок"));

        var innerEmphasisToken = new Token(TokenType.Emphasis, string.Empty);
        innerEmphasisToken.Children.Add(new Token(TokenType.Text, " полужирный курсив"));

        var outerHeaderToken = new Token(TokenType.Header, string.Empty);
        outerHeaderToken.Children.Add(innerStrongToken);

        var outerStrongToken = new Token(TokenType.Strong, string.Empty);
        outerStrongToken.Children.Add(new Token(TokenType.Text, " и "));
        outerStrongToken.Children.Add(innerEmphasisToken);

        var tokens = new List<Token>
        {
            outerHeaderToken,
            outerStrongToken,
        };

        var result = renderer.Render(tokens);

        result.Should().Be("<h1><strong>полужирный заголовок</strong></h1>" +
                           "<strong> и <em> полужирный курсив</em></strong>");
    }
}