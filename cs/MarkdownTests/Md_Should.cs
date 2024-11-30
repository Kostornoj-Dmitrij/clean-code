using FluentAssertions;
using Markdown;
using NUnit.Framework;
using System.Diagnostics;
using Markdown.Parsers;
using Markdown.Renderers;

namespace MarkdownTests;

[TestFixture]
public class Md_Should
{
    private HtmlRenderer renderer;
    private MarkdownParser parser;
    private Md md;

    [SetUp]
    public void Setup()
    {
        renderer = new HtmlRenderer();
        parser = new MarkdownParser();
        md = new Md(renderer, parser);
    }


    [Test]
    public void Md_ShouldThrowArgumentNullException_WhenInputIsNull()
    {
        var func = () => md.Render(null!);
        func.Should().Throw<ArgumentNullException>();
    }

    [TestCase("", "", TestName = "InputIsEmpty")]
    [TestCase("Это # не заголовок", 
        "Это # не заголовок", 
        TestName = "InvalidHeaderTags")]
    [TestCase(@"Здесь сим\волы экранирования\ \должны остаться.\", 
        @"Здесь сим\волы экранирования\ \должны остаться.\", 
        TestName = "EscapingSymbols")]
    [TestCase("Это н_е_ будет _ вы_деле_но", 
        "Это н_е_ будет _ вы_деле_но", 
        TestName = "InvalidItalicTags")]
    [TestCase("Это н__е__ будет __ вы__деле__но", 
        "Это н__е__ будет __ вы__деле__но", 
        TestName = "InvalidStrongTags")]
    [TestCase("В ра_зных сл_овах", 
        "В ра_зных сл_овах", 
        TestName = "TagsInDifferentWords")]
    [TestCase("Это текст_с_подчеркиваниями_12_3", 
        "Это текст_с_подчеркиваниями_12_3", 
        TestName = "UnderscoresInsideWords")]
    [TestCase("Это __непарные_ символы в одном абзаце.", 
        "Это __непарные_ символы в одном абзаце.", 
        TestName = "UnclosedTags")]
    [TestCase("Если пустая _______ строка", 
        "Если пустая _______ строка", 
        TestName = "EmptyTags")]
    public void Md_ShouldNotRender_When(string input, string expected)
    {
        var result = md.Render(input);

        result.Should().Be(expected);
    }

    [TestCase("Это _курсив_ и __полужирный__ текст", 
        "Это <em>курсив</em> и <strong>полужирный</strong> текст", 
        TestName = "ItalicAndStrongTags")]
    [TestCase("# Заголовок", 
        "<h1>Заголовок</h1>", 
        TestName = "HeaderTags")]
    [TestCase("# Заголовок __с _разными_ символами__", 
        "<h1>Заголовок <strong>с <em>разными</em> символами</strong></h1>", 
        TestName = "HeaderWithNestedTags")]
    [TestCase("Это __полужирный _текст_, _с курсивом_ внутри__", 
        "Это <strong>полужирный <em>текст</em>, <em>с курсивом</em> внутри</strong>", 
        TestName = "ItalicInStrong")]
    [TestCase("Это _курсив с __полужирным__ внутри_", 
        "Это <em>курсив с __полужирным__ внутри</em>", 
        TestName = "StrongInItalic")]
    [TestCase(@"Экранированный \_символ\_", 
        "Экранированный _символ_", 
        TestName = "EscapeTag")]
    [TestCase(@"\\_вот это будет выделено тегом_", 
        "<em>вот это будет выделено тегом</em>", 
        TestName = "EscapedYourselfOnStartOfTag")]
    [TestCase(@"_e\\_", 
        "<em>e</em>", 
        TestName = "EscapedYourselfOnEndOfTag")]
    [TestCase("# Заголовок 1\n# Заголовок 2", 
        "<h1>Заголовок 1</h1>\n<h2>Заголовок 2</h2>", 
        TestName = "MultipleHeaders")]
    [TestCase("# h __s _E _e_ E_ s__ _e_", 
        "<h1>h <strong>s <em>E <em>e</em> E</em> s</strong> <em>e</em></h1>", 
        TestName = "LotNestedTags")]
    [TestCase("_e __s e_ s__", 
        "_e __s e_ s__", 
        TestName = "TagsIntersection")]
    [TestCase("en_d._, mi__dd__le, _sta_rt", 
        "en<em>d.</em>, mi<strong>dd</strong>le, <em>sta</em>rt", 
        TestName = "BoundedTagsInOneWord")]
    [TestCase("__s \n s__, _e \r\n e_", 
        "__s \n s__, _e \r\n e_", 
        TestName = "NewLines")]
    [TestCase("_e __e", 
        "_e __e", 
        TestName = "UnPairedTags2")]
    public void Md_ShouldRender_When(string input, string expected)
    {
        var result = md.Render(input);

        result.Should().Be(expected);
    }

    [Test]
    public void Md_ShouldRenderLargeInputQuickly()
    {
        var largeInput = string.Concat(Enumerable.Repeat("_Пример_ ", 10000));
        var expectedOutput = string.Concat(Enumerable.Repeat("<em>Пример</em> ", 10000));
        var stopwatch = new Stopwatch();

        stopwatch.Start();
        var result = md.Render(largeInput);
        stopwatch.Stop();

        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000);
        expectedOutput.Should().BeEquivalentTo(result);
    }
}