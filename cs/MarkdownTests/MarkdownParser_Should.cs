using FluentAssertions;
using Markdown.Parsers;
using Markdown.Tokens;
using NUnit.Framework;

namespace MarkdownTests;

[TestFixture]
public class MarkdownParser_Should
{
    [Test]
    public void MarkdownParser_ShouldParse_WhenItalicTag()
    {
        var tokens = MarkdownParser
            .ParseTokens("Это _курсив_ текст").ToList();

        tokens.Should().HaveCount(3);
        tokens[0].Type.Should().Be(TokenType.Text);
        tokens[0].Content.Should().Be("Это ");
        tokens[1].Type.Should().Be(TokenType.Emphasis);
        tokens[1].Children[0].Content.Should().Be("курсив");
        tokens[2].Type.Should().Be(TokenType.Text);
        tokens[2].Content.Should().Be(" текст");
    }

    [Test]
    public void MarkdownParser_ShouldParse_WhenStrongTag()
    {
        var tokens = MarkdownParser
            .ParseTokens("Это __полужирный__ текст").ToList();

        tokens.Should().HaveCount(3);
        tokens[0].Type.Should().Be(TokenType.Text);
        tokens[0].Content.Should().Be("Это ");
        tokens[1].Type.Should().Be(TokenType.Strong);
        tokens[1].Children[0].Type.Should().Be(TokenType.Text);
        tokens[1].Children[0].Content.Should().Be("полужирный");
        tokens[2].Type.Should().Be(TokenType.Text);
        tokens[2].Content.Should().Be(" текст");
    }

    [Test]
    public void MarkdownParser_ShouldParse_WhenHeaderTag()
    {
        var tokens = MarkdownParser
            .ParseTokens("# Заголовок").ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Type.Should().Be(TokenType.Header);
        tokens[0].Children[0].Content.Should().Be("Заголовок");
    }

    [Test]
    public void MarkdownParser_ShouldParse_WhenEscaping()
    {
        var tokens = MarkdownParser
            .ParseTokens(@"Экранированный \_символ\_").ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Type.Should().Be(TokenType.Text);
        tokens[0].Content.Should().Be("Экранированный _символ_");
    }

    [Test]
    public void MarkdownParser_ShouldParse_WhenNestedItalicAndStrongTags()
    {
        var tokens = MarkdownParser
            .ParseTokens("Это __жирный _и курсивный_ текст__").ToList();

        tokens.Should().HaveCount(2);
        tokens[0].Type.Should().Be(TokenType.Text);
        tokens[0].Content.Should().Be("Это ");
        tokens[1].Type.Should().Be(TokenType.Strong);
        tokens[1].Children.Should().HaveCount(3);
        tokens[1].Children[0].Type.Should().Be(TokenType.Text);
        tokens[1].Children[0].Content.Should().Be("жирный ");
        tokens[1].Children[1].Type.Should().Be(TokenType.Emphasis);
        tokens[1].Children[1].Children[0].Type.Should().Be(TokenType.Text);
        tokens[1].Children[1].Children[0].Content.Should().Be("и курсивный");
        tokens[1].Children[2].Type.Should().Be(TokenType.Text);
        tokens[1].Children[2].Content.Should().Be(" текст");
    }

    [Test]
    public void MarkdownParser_ShouldParse_WhenMultipleTokensInLine()
    {
        var tokens = MarkdownParser
            .ParseTokens("Это _курсив_, а это __жирный__ текст.").ToList();

        tokens.Should().HaveCount(5);
        tokens[0].Type.Should().Be(TokenType.Text);
        tokens[0].Content.Should().Be("Это ");
        tokens[1].Type.Should().Be(TokenType.Emphasis);
        tokens[1].Children[0].Content.Should().Be("курсив");
        tokens[2].Type.Should().Be(TokenType.Text);
        tokens[2].Content.Should().Be(", а это ");
        tokens[3].Type.Should().Be(TokenType.Strong);
        tokens[3].Children[0].Content.Should().Be("жирный");
        tokens[4].Type.Should().Be(TokenType.Text);
        tokens[4].Content.Should().Be(" текст.");
    }

    [Test]
    public void MarkdownParser_ShouldNotParse_WhenEscapingSymbols()
    {
        var tokens = MarkdownParser
            .ParseTokens(@"Здесь сим\волы экранирования\ \должны остаться.\").ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Type.Should().Be(TokenType.Text);
        tokens[0].Content.Should().Be(@"Здесь сим\волы экранирования\ \должны остаться.\");
    }

    [Test]
    public void MarkdownParser_ShouldParse_WhenEscapedTags()
    {
        var tokens = MarkdownParser
            .ParseTokens(@"\\_вот это будет выделено тегом_").ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Type.Should().Be(TokenType.Emphasis);
        tokens[0].Children[0].Content.Should().Be("вот это будет выделено тегом");
    }

    [Test]
    public void MarkdownParser_ShouldParse_WhenNestedItalicAndStrongCorrectly()
    {
        var tokens = MarkdownParser
            .ParseTokens("Это __двойное _и одинарное_ выделение__").ToList();

        tokens.Should().HaveCount(2);
        tokens[0].Type.Should().Be(TokenType.Text);
        tokens[0].Content.Should().Be("Это ");
        tokens[1].Type.Should().Be(TokenType.Strong);
        tokens[1].Children.Should().HaveCount(3);
        tokens[1].Children[0].Type.Should().Be(TokenType.Text);
        tokens[1].Children[0].Content.Should().Be("двойное ");
        tokens[1].Children[1].Type.Should().Be(TokenType.Emphasis);
        tokens[1].Children[1].Children[0].Type.Should().Be(TokenType.Text);
        tokens[1].Children[1].Children[0].Content.Should().Be("и одинарное");
        tokens[1].Children[2].Type.Should().Be(TokenType.Text);
        tokens[1].Children[2].Content.Should().Be(" выделение");
    }

    [Test]
    public void MarkdownParser_ShouldParse_WhenHeaderWithTags()
    {
        var tokens = MarkdownParser
            .ParseTokens("# Заголовок __с _разными_ символами__").ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Type.Should().Be(TokenType.Header);
        tokens[0].Children.Should().HaveCount(2);
        tokens[0].Children[0].Type.Should().Be(TokenType.Text);
        tokens[0].Children[0].Content.Should().Be("Заголовок ");
        tokens[0].Children[1].Type.Should().Be(TokenType.Strong);
        tokens[0].Children[1].Children[0].Type.Should().Be(TokenType.Text);
        tokens[0].Children[1].Children[0].Content.Should().Be("с ");
        tokens[0].Children[1].Children[1].Children[0].Type.Should().Be(TokenType.Text);
        tokens[0].Children[1].Children[1].Children[0].Content.Should().Be("разными");
        tokens[0].Children[1].Children[2].Type.Should().Be(TokenType.Text);
        tokens[0].Children[1].Children[2].Content.Should().Be(" символами");
    }

    [Test]
    public void MarkdownParser_ShouldNotParse_WhenEmptyEmphasis()
    {
        var tokens = MarkdownParser
            .ParseTokens("Если пустая _______ строка").ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Type.Should().Be(TokenType.Text);
        tokens[0].Content.Should().Be("Если пустая _______ строка");
    }

    [Test]
    public void MarkdownParser_ShouldParse_WhenMultipleHeaders()
    {
        var tokens = MarkdownParser
            .ParseTokens("# Заголовок 1\n# Заголовок 2").ToList();

        tokens.Should().HaveCount(3);
        tokens[0].Type.Should().Be(TokenType.Header);
        tokens[0].Children[0].Content.Should().Be("Заголовок 1");
        tokens[2].Type.Should().Be(TokenType.Header);
        tokens[2].Children[0].Content.Should().Be("Заголовок 2");
    }

    [Test]
    public void MarkdownParser_ShouldNotParse_WhenUnderscoresInNumbers()
    {
        var tokens = MarkdownParser
            .ParseTokens("Текст с цифрами_12_3 не должен выделяться").ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Type.Should().Be(TokenType.Text);
        tokens[0].Content.Should().Be("Текст с цифрами_12_3 не должен выделяться");
    }

    [Test]
    public void MarkdownParser_ShouldNotParse_WhenTagsInWords()
    {
        var tokens = MarkdownParser
            .ParseTokens("и в _нач_але, и в сер_еди_не").ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Type.Should().Be(TokenType.Text);
        tokens[0].Content.Should().Be("и в _нач_але, и в сер_еди_не");
    }

    [Test]
    public void MarkdownParser_ShouldNotParse_WhenDifferentWords()
    {
        var tokens = MarkdownParser
            .ParseTokens("Это пер_вый в_торой пример.").ToList();

        tokens.Should().HaveCount(1);
        tokens[0].Type.Should().Be(TokenType.Text);
        tokens[0].Content.Should().Be("Это пер_вый в_торой пример.");
    }
}