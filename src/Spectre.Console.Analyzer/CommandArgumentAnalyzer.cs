using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Spectre.Console.Cli;

namespace Spectre.Console.Analyzer
{
    /// <summary>
    /// Analyzes method parameters with applied
    /// verifying that the value is well-formed.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CommandArgumentAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        /// Name of the analyzer.
        /// </summary>
        public const string DiagnosticId = "Spectre100";

        private const string Title = "Command arguments are formatted correctly";
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
                compilationStartAnalysisContext.RegisterSyntaxNodeAction(
                    context =>
                    {
                        var attribute = (AttributeSyntax)context.Node;
                        if (attribute.Name.ToString() != "CommandArgument")
                        {
                            return;
                        }

                        var literal = attribute.ArgumentList.Arguments[1].Expression as LiteralExpressionSyntax;
                        if (literal == null)
                        {
                            // only support literals for now I guess
                            return;
                        }

                        var value = (string)literal.Token.Value;

                        try
                        {
                            TemplateParser.ParseArgumentTemplate(value);
                        }
                        catch (Exception ex)
                        {
                            context.ReportDiagnostic(
                                Diagnostic.Create(
                                    Rule,
                                    context.Node.GetLocation(), ex.Message));
                        }
                    }, SyntaxKind.Attribute);
            });
        }

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }
    }
}