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
    internal static class SuggestionBrowser
    {
        private static readonly List<Suggestion> _suggestions = Downloader.GetLocalSuggestions();

        /// <summary>
        /// Finds a specific suggestion based on its R# name. If nothing was found, returns <c>null</c>.
        /// </summary>
        /// <param name="suggestionName">R# type name of the suggestion.</param>
        /// <returns></returns>
        internal static Suggestion GetSuggestion(string suggestionName)
        {
        	var result = _suggestions.Find(i => i.ResharperName == suggestionName);
			if (result != null)
				return result;

        	foreach (var suggestion in _suggestions) {
        		foreach (var alias in suggestion.ResharperNameAliases) {
					if (alias == suggestionName)
						return suggestion;
        		}
        	}
            return null;
        }
    }
}
