namespace Spectre.Console
{
    /// <summary>
    /// Represents (read-only) console capabilities.
    /// </summary>
    public interface IReadOnlyCapabilities
    {
        /// <summary>
        /// Gets a value indicating whether or not
        /// the console supports Ansi.
        /// </summary>
        bool Ansi { get; }

        /// <summary>
        /// Gets a value indicating whether or not
        /// the console support links.
        /// </summary>
        bool Links { get; }

        /// <summary>
        /// Gets a value indicating whether or not
        /// this is a legacy console (cmd.exe) on an OS
        /// prior to Windows 10.
        /// </summary>
        /// <remarks>
        /// Only relevant when running on Microsoft Windows.
        /// </remarks>
        bool Legacy { get; }

        /// <summary>
        /// Gets a value indicating whether or not
        /// console output has been redirected.
        /// </summary>
        bool Tty { get; }

        /// <summary>
        /// Gets a value indicating whether
        /// or not the console supports interaction.
        /// </summary>
        bool Interactive { get; }

        /// <summary>
        /// Gets a value indicating whether
        /// or not the console supports Unicode.
        /// </summary>
        bool Unicode { get; }
    }
}
