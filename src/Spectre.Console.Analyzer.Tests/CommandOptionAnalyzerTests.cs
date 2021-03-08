using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Spectre.Console.Analyzer.Tests
{
    public class CommandOptionAnalyzerTests
    {
        [Fact]
        public async void Values_with_optional_are_good()
        {
            const string Source =
                @"
using Spectre.Console.Cli;

class MySettings : CommandSettings
{
    [CommandOption(""d|dd"")]
    public string Option { get;set; }
}";
            var result = new DiagnosticResult(
                    CommandOptionsAnalyzer.DiagnosticId,
                    DiagnosticSeverity.Warning)
                .WithLocation(6, 6);
            
            await SpectreAnalyzerVerifier<CommandOptionsAnalyzer>.VerifyAnalyzerAsync(Source, result)
                .ConfigureAwait(false);
        }

    }
}