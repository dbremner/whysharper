using System;
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
		/// R# backwards/forwards compatibile name. Internal names can change, and we can track that
		/// with aliases. E.g.: R#4.0 has UnusedUsingDirectiveError but R#4.5 uses UnusedUsingDirectiveWarning instead.
		/// </summary>
		public List<string> ResharperNameAliases { get; private set; }

        /// <summary>
        /// List of "name/link" pairs that gets displayed in a popup for this very suggestion.
        /// </summary>
        public List<KeyValuePair<string, string>> Links { get; private set; }

        /// <summary>
        /// Creates a new suggestion item.
        /// </summary>
        /// <param name="resharperName">R# type name.</param>
        /// <param name="aliases">Names from previous (or future) R# versions that are aliases of the main one.</param>
        internal Suggestion(string resharperName, string aliases)
        {
            ResharperName = resharperName;
            Links = new List<KeyValuePair<string, string>>();

			ResharperNameAliases = new List<string>();
            foreach (var alias in aliases.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)) {
				ResharperNameAliases.Add(alias);
        	}
        }
    }
}
