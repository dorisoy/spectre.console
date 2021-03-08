using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace Spectre.Console.Analyzer.Tests
{
    public static class SpectreAnalyzerVerifier<TAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        public static Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
        {
            var test = new Test
            {
                TestCode = source,
                CompilerDiagnostics = CompilerDiagnostics.All,
                ReferenceAssemblies = ReferenceAssemblies.Net.Net50,
                TestState = { AdditionalReferences = { typeof(AnsiConsole).Assembly.Location } },
                TestBehaviors = TestBehaviors.SkipGeneratedCodeCheck,
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync();
        }

        // Code fix tests support both analyzer and code fix testing. This test class is derived from the code fix test
        // to avoid the need to maintain duplicate copies of the customization work.
        public class Test : CSharpCodeFixTest<TAnalyzer, EmptyCodeFixProvider, XUnitVerifier>
        {
        }
    }
}