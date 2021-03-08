using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using Verify = Spectre.Console.Analyzer.Tests.SpectreAnalyzerVerifier<Spectre.Console.Analyzer.MarkupFormatAnalyzer>;

namespace Spectre.Console.Analyzer.Tests
{
    public class MarkupAnalyzerTests
    {
        [Fact]
        public async void Valid_markup_reports_no_warnings()
        {
            const string Source =
                @"
using Spectre.Console;

class TestClass { void TestMethod() {
    AnsiConsole.MarkupLine(""[red]Should be red[/]"");
} }";

            await Verify.VerifyAnalyzerAsync(Source)
                .ConfigureAwait(false);
        }

        [Fact]
        public async void Strings_with_missing_end_tag_are_reported_as_a_warning()
        {
            const string Source =
                @"
using Spectre.Console;

class TestClass { void TestMethod() {
    AnsiConsole.MarkupLine(""[red]Should be red"");
} }";

            var result = new DiagnosticResult(
                    MarkupFormatAnalyzer.DiagnosticId,
                    DiagnosticSeverity.Warning)
                .WithLocation(5, 28);

            await Verify.VerifyAnalyzerAsync(Source, result)
                .ConfigureAwait(false);
        }

        [Fact]
        public async void Bad_colors_have_warnings()
        {
            const string Source =
                @"
using Spectre.Console;

class TestClass { void TestMethod() {
    AnsiConsole.MarkupLine(""[lighturple]Should be red[/]"");
} }";

            var result = new DiagnosticResult(
                    MarkupFormatAnalyzer.DiagnosticId,
                    DiagnosticSeverity.Warning)
                .WithLocation(5, 28);

            await Verify.VerifyAnalyzerAsync(Source, result)
                .ConfigureAwait(false);
        }
    }
}