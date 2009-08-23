using System.Collections.Generic;

namespace WhySharper
{
    /// <summary>
    /// Represents a WhySharper suggestion.
    /// </summary>
    internal class Suggestion
    {
        /// <summary>
        /// R# type name of the error/warning/suggestion (eg, MemberCanBeMadeStaticLocalWarning, 
        /// ConvertToConstantLocalWarning, MethodDeclarationInEnumError, etc, etc).
        /// </summary>
        public string ResharperName { get; private set; }

        /// <summary>
        /// List of "name/link" pairs that gets displayed in a popup for this very suggestion.
        /// </summary>
        public List<KeyValuePair<string, string>> Links { get; private set; }

        /// <summary>
        /// Creates a new suggestion item.
        /// </summary>
        /// <param name="resharperName">R# type name.</param>
        internal Suggestion(string resharperName)
        {
            ResharperName = resharperName;
            Links = new List<KeyValuePair<string, string>>();
        }
    }
}
