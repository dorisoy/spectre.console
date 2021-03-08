using System.Collections.Generic;

namespace Spectre.Console.Cli
{
    /// <summary>
    /// Parses command line templates
    /// </summary>
    public static class TemplateParser
    {
        /// <summary>
        /// Represents an argument.
        /// </summary>
        public sealed class ArgumentResult
        {
            public string Value { get; set; }
            public bool Required { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="ArgumentResult"/> class.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="required">If it's required.</param>
            public ArgumentResult(string value, bool required)
            {
                Value = value;
                Required = required;
            }
        }

        public sealed class OptionResult
        {
            public List<string> LongNames { get; set; }
            public List<string> ShortNames { get; set; }
            public string? Value { get; set; }
            public bool ValueIsOptional { get; set; }

            public OptionResult()
            {
                ShortNames = new List<string>();
                LongNames = new List<string>();
            }
        }

        /// <summary>
        /// Parses a template.
        /// </summary>
        /// <param name="template">The template to parse.</param>
        /// <returns>A parsed argument.</returns>
        public static ArgumentResult ParseArgumentTemplate(string template)
        {
            var valueName = default(string);
            var required = false;
            foreach (var token in TemplateTokenizer.Tokenize(template))
            {
                if (token.TokenKind == TemplateToken.Kind.ShortName ||
                    token.TokenKind == TemplateToken.Kind.LongName)
                {
                    throw CommandTemplateException.ArgumentCannotContainOptions(template, token);
                }

                if (token.TokenKind == TemplateToken.Kind.OptionalValue ||
                    token.TokenKind == TemplateToken.Kind.RequiredValue)
                {
                    if (!string.IsNullOrWhiteSpace(valueName))
                    {
                        throw CommandTemplateException.MultipleValuesAreNotSupported(template, token);
                    }

                    if (string.IsNullOrWhiteSpace(token.Value))
                    {
                        throw CommandTemplateException.ValuesMustHaveName(template, token);
                    }

                    valueName = token.Value;
                    required = token.TokenKind == TemplateToken.Kind.RequiredValue;
                }
            }

            if (valueName == null)
            {
                throw CommandTemplateException.ArgumentsMustHaveValueName(template);
            }

            return new ArgumentResult(valueName, required);
        }

        /// <summary>
        /// Parses an option template.
        /// </summary>
        /// <param name="template">The template to parse.</param>
        /// <returns>The parsed option.</returns>
        public static OptionResult ParseOptionTemplate(string template)
        {
            var result = new OptionResult();

            foreach (var token in TemplateTokenizer.Tokenize(template))
            {
                if (token.TokenKind == TemplateToken.Kind.LongName || token.TokenKind == TemplateToken.Kind.ShortName)
                {
                    if (string.IsNullOrWhiteSpace(token.Value))
                    {
                        throw CommandTemplateException.OptionsMustHaveName(template, token);
                    }

                    if (char.IsDigit(token.Value[0]))
                    {
                        throw CommandTemplateException.OptionNamesCannotStartWithDigit(template, token);
                    }

                    foreach (var character in token.Value)
                    {
                        if (!char.IsLetterOrDigit(character) && character != '-' && character != '_')
                        {
                            throw CommandTemplateException.InvalidCharacterInOptionName(template, token, character);
                        }
                    }
                }

                if (token.TokenKind == TemplateToken.Kind.LongName)
                {
                    if (token.Value.Length == 1)
                    {
                        throw CommandTemplateException.LongOptionMustHaveMoreThanOneCharacter(template, token);
                    }

                    result.LongNames.Add(token.Value);
                }

                if (token.TokenKind == TemplateToken.Kind.ShortName)
                {
                    if (token.Value.Length > 1)
                    {
                        throw CommandTemplateException.ShortOptionMustOnlyBeOneCharacter(template, token);
                    }

                    result.ShortNames.Add(token.Value);
                }

                if (token.TokenKind == TemplateToken.Kind.RequiredValue ||
                    token.TokenKind == TemplateToken.Kind.OptionalValue)
                {
                    if (!string.IsNullOrWhiteSpace(result.Value))
                    {
                        throw CommandTemplateException.MultipleOptionValuesAreNotSupported(template, token);
                    }

                    foreach (var character in token.Value)
                    {
                        if (!char.IsLetterOrDigit(character) &&
                            character != '=' && character != '-' && character != '_')
                        {
                            throw CommandTemplateException.InvalidCharacterInValueName(template, token, character);
                        }
                    }

                    result.Value = token.Value.ToUpperInvariant();
                    result.ValueIsOptional = token.TokenKind == TemplateToken.Kind.OptionalValue;
                }
            }

            if (result.LongNames.Count == 0 &&
                result.ShortNames.Count == 0)
            {
                throw CommandTemplateException.MissingLongAndShortName(template, null);
            }

            return result;
        }
    }
}
