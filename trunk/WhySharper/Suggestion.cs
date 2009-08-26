/// Copyright 2009 Andrew Kazyrevich, http://codevanced.net
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); 
/// you may not use this file except in compliance with the License. 
/// You may obtain a copy of the License at 
/// 
/// http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software 
/// distributed under the License is distributed on an "AS IS" BASIS, 
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
/// See the License for the specific language governing permissions and 
/// limitations under the License. 
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
