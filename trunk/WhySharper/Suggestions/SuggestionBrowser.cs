using System;
using System.Collections.Generic;
using System.Xml.Linq;
using JetBrains.Util;

namespace WhySharper.Suggestions
{
    internal static class SuggestionBrowser
    {
        private static readonly List<Suggestion> _suggestions = LoadSuggestions();
        private const string SuggestionsFile = "Suggestions.xml";

        /// <summary>
        /// Loads all suggestions from xml.
        /// </summary>
        /// <returns></returns>
        private static List<Suggestion> LoadSuggestions()
        {
            try {
                return LoadSuggestionsCore();
            }
            catch (Exception ex) {
                Logger.LogException(ex);
                return new List<Suggestion>();
            }
        }

        /// <summary>
        /// Finds a specific suggestion based on its R# name. If nothing was found, returns <c>null</c>.
        /// </summary>
        /// <param name="suggestionName">R# type name of the suggestion.</param>
        /// <returns></returns>
        internal static Suggestion GetSuggestion(string suggestionName)
        {
            return _suggestions.Find(i => i.ResharperName == suggestionName);
        }

        private static List<Suggestion> LoadSuggestionsCore() 
        {
            var result = new List<Suggestion>();

            var file = XDocument.Load(SuggestionsFile);
            foreach (var suggestion in file.Descendants("suggestion")) {
                var name = suggestion.Attribute("name");
                if (name == null) continue;

                var item = new Suggestion(name.Value);
                foreach (var link in suggestion.Elements("links").Elements("link")) {
                    var linkName = link.Attribute("name");
                    if (linkName != null) {
                        item.Links.Add(new KeyValuePair<string, string>(linkName.Value, link.Value));
                    }
                }
                result.Add(item);
            }

            return result;
        }
    }
}
