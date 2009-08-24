using System;
using System.Collections.Generic;

namespace WhySharper
{
    internal static class SuggestionBrowser
    {
        internal static readonly string Folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\JetBrains\\WhySharper";
        internal static readonly string File = string.Format("{0}\\Suggestions.xml", Folder);

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
