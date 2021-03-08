using System;

namespace Spectre.Console
{
    /// <summary>
    /// Specifies the method parameter is markdown text.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class MarkupTextAttribute : Attribute
    {
    }
}