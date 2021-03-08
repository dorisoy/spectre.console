using System;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Spectre.Console.Analyzer
{
    /// <summary>
    /// Analyzes method parameters with applied
    /// verifying that the value is well-formed.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MarkupFormatAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// Name of the analyzer.
        /// </summary>
        public const string DiagnosticId = "Spectre001";

        private const string Title = "Markup is formatted correctly";
        private const string MessageFormat = "{0}";
        private const string Description = "Ensure your string has a valid format";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat,
            Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext analysisContext)
        {
            analysisContext.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.ReportDiagnostics);
            analysisContext.EnableConcurrentExecution();
            analysisContext.RegisterCompilationStartAction(compilationStartAnalysisContext =>
            {
                var markupTextAttribute = compilationStartAnalysisContext.Compilation.GetTypeByMetadataName("Spectre.Console.MarkupTextAttribute");
                if (markupTextAttribute == null)
                {
                    return;
                }

                compilationStartAnalysisContext.RegisterOperationAction(
                    operationAnalysisContext =>
                    {
                        var argumentOperation = (IArgumentOperation)operationAnalysisContext.Operation;
                        if (!argumentOperation.Parameter.GetAttributes().ContainsAttributeType(markupTextAttribute))
                        {
                            return;
                        }

                        if (!argumentOperation.Value.ConstantValue.HasValue)
                        {
                            return;
                        }

                        try
                        {
                            MarkupParser.Parse((string)argumentOperation.Value.ConstantValue.Value);
                        }
                        catch (InvalidOperationException ex)
                        {
                            // probably should have a different rule per error that could be encountered
                            // but one for now works
                            operationAnalysisContext.ReportDiagnostic(
                                Diagnostic.Create(
                                    Rule,
                                    operationAnalysisContext.Operation.Syntax.GetLocation(), ex.Message));
                        }
                    }, OperationKind.Argument);
            });
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }
    }
}