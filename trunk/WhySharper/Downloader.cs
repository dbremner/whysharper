using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using JetBrains.Application;
using JetBrains.ComponentModel;
using JetBrains.Util;

namespace WhySharper
{
    [ShellComponentInterface(ProgramConfigurations.VS_ADDIN)]
    [ShellComponentImplementation]
    public partial class Downloader : IXmlExternalizableShellComponent
    {
        private const string SuggestionsXmlSource = "http://whysharper.googlecode.com/svn/trunk/WhySharper/Suggestions.xml";
        private static readonly object _sync = new object();
        private static bool _updated;

        /// <summary>
        /// Returns suggestions from local suggestions.xml - this one should be already overwritten
        /// with the latest version from google code.
        /// </summary>
        /// <returns></returns>
        internal static List<Suggestion> GetLocalSuggestions()
        {
            UpdateSuggestionsXml();
            return GetSuggestions();
        }
        
        /// <summary>
        /// Gets called before any solution is loaded. Here we download the the latest version 
        /// of suggestions.xml from google code and rewrite the local xml.
        /// </summary>
        public void Init()
        {
            UpdateSuggestionsXml();
        }

        /// <summary>
        /// Pairing method to <see cref="Init"/> that's guaranteed to be called by the component container 
        /// to tear down your component. Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// 
        /// All the components you access from <see cref="Init"/> are guaranteed to exist when <see cref="Dispose"/> 
        /// is called. Any other components might be missing, and trying to access them will throw an exception.
        /// </summary>
        public void Dispose()
        {
        }

        private static List<Suggestion> GetSuggestions()
        {
            var result = new List<Suggestion>();

            try {
                XDocument file;
                lock (_sync) {
                    file = XDocument.Load(SuggestionBrowser.File);
                }

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
            }
            catch (Exception ex) {
                Logger.LogException("WhySharper failed to load suggestions xml from " + SuggestionBrowser.File, ex);
                return new List<Suggestion>();
            }
            return result;
        }

        private static void UpdateSuggestionsXml()
        {
            if (_updated)
                return;

            try {
                var content = DownloadSuggestions(SuggestionsXmlSource);
                lock (_sync) {
                    using (var writer = new StreamWriter(SuggestionBrowser.File, false)) {
                        writer.Write(content);
                    }
                }
            }
            catch (Exception ex) {
                const string message = "WhySharper failed to download the latest xml from {0} and save it to {1}.";
                Logger.LogException(string.Format(message, SuggestionsXmlSource, SuggestionBrowser.File), ex);
            }
            finally {
                //Mark it as updated in any case: we don't want to put unnecessary pressure on VS
                _updated = true;
            }
        }

        private static string DownloadSuggestions(string source)
        {
            var content = new StringBuilder();

            using (var stream = new WebClient().OpenRead(source)) {
                var reader = new StreamReader(stream);
                string line = reader.ReadLine();
                while (line != null) {
                    content.AppendLine(line);
                    line = reader.ReadLine();
                }
            }

            return content.ToString();
        }
    }
}
