using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace Spectre.Console.Analyzer.Tests
{
    public class CommandArgumentAnalyzerTests
    {
        [Fact]
        public async void Values_with_optional_are_good()
        {
            const string Source =
                @"
using Spectre.Console.Cli;

class MySettings : CommandSettings
{
    [CommandArgument(0, ""[OPTIONAL]"")]
    public string Value { get;set; }
}";

            await SpectreAnalyzerVerifier<CommandArgumentAnalyzer>.VerifyAnalyzerAsync(Source)
                .ConfigureAwait(false);
        }
        
        [Fact]
        public async void Values_with_required_are_good()
        {
            const string Source =
                @"
using Spectre.Console.Cli;

class MySettings : CommandSettings
{
    [CommandArgument(0, ""<REQUIRED>"")]
    public string Value { get;set; }
}";

            await SpectreAnalyzerVerifier<CommandArgumentAnalyzer>.VerifyAnalyzerAsync(Source)
                .ConfigureAwait(false);
        }
        
        [Fact]
        public async void Values_with_bad_format_warn()
        {
            const string Source =
                @"
using Spectre.Console.Cli;

class MySettings : CommandSettings
{
    [CommandArgument(0, ""OPPS"")]
    public string Value { get;set; }
}";

            var result = new DiagnosticResult(
                    CommandArgumentAnalyzer.DiagnosticId,
                    DiagnosticSeverity.Warning)
                .WithLocation(6, 6);
            
            await SpectreAnalyzerVerifier<CommandArgumentAnalyzer>.VerifyAnalyzerAsync(Source, result)
                .ConfigureAwait(false);
        }
    }
}