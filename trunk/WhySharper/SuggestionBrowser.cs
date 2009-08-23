using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace WhySharper
{
    internal static class SuggestionBrowser
    {
        internal static readonly string File = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Suggestions.xml";

        private static readonly List<Suggestion> _suggestions = Downloader.GetLocalSuggestions();

        /// <summary>
        /// Finds a specific suggestion based on its R# name. If nothing was found, returns <c>null</c>.
        /// </summary>
        /// <param name="suggestionName">R# type name of the suggestion.</param>
        /// <returns></returns>
        internal static Suggestion GetSuggestion(string suggestionName)
        {
            return _suggestions.Find(i => i.ResharperName == suggestionName);
        }
    }
}
