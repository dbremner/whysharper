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
using System.IO;
using System.Net;
using System.Text;
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
        private const string RemoteXmlFile = "http://whysharper.googlecode.com/svn/trunk/WhySharper/Suggestions.xml";
        private const string RemoteVersionFile = "http://whysharper.googlecode.com/svn/trunk/WhySharper/SuggestionsVersion.txt";

        private static readonly string Folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\JetBrains\\WhySharper";
        private static readonly string XmlFile = Path.Combine(Folder, "Suggestions.xml");
        private static readonly string VersionFile = Path.Combine(Folder, "SuggestionsVersion.txt");

        private static readonly object _sync = new object();
        private static bool _updated;

        /// <summary>
        /// Updates the local suggestions xml and returns the list of suggestions from it.
        /// </summary>
        /// <returns></returns>
        internal static List<Suggestion> GetLocalSuggestions()
        {
            UpdateSuggestionsXml();
            return ParseSuggestions();
        }
        
        private static List<Suggestion> ParseSuggestions()
        {
            var result = new List<Suggestion>();

            try {
                XDocument file;
                lock (_sync) {
                    file = XDocument.Load(XmlFile);
                }

                foreach (var suggestion in file.Descendants("suggestion")) {
                    var name = suggestion.Attribute("name");
					if (name == null) continue;

					var aliases = suggestion.Attribute("aliases") ?? new XAttribute("aliases", "");
					var item = new Suggestion(name.Value, aliases.Value);
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
                Logger.LogException("WhySharper failed to load suggestions xml from " + XmlFile, ex);
                return new List<Suggestion>();
            }
            return result;
        }

        private static void UpdateSuggestionsXml()
        {
            if (_updated || IsUpToDate())
                return;

            var content = DownloadFrom(RemoteXmlFile);

            try {
                lock (_sync) {
                    EnsureFolderExists();
                    using (var writer = new StreamWriter(XmlFile, false)) {
                        writer.Write(content);
                    }
                }
            }
            catch (Exception ex) {
                Logger.LogException("WhySharper failed to save the latest xml to " +  XmlFile, ex);
            }
            finally {
                //Mark it as updated in any case: we don't want to put unnecessary pressure on VS
                _updated = true;
            }
        }

        /// <summary>
        /// Returns true if local version is the same or above the remote one 
        /// (in which case we don't want to download suggestions.xml).
        /// </summary>
        /// <returns></returns>
        private static bool IsUpToDate()
        {
            int? remoteVersion = ParseVersion(false);
            if (remoteVersion == null) {
                return true; //as we probably wouldn't be able to download the xml as well
            }

            int? localVersion = ParseVersion(true);
            if (localVersion == null || localVersion < remoteVersion) {
                WriteVersion(remoteVersion.Value);
                return false;
            }

            return true;
        }

        private static int? ParseVersion(bool isLocal)
        {
            string content = "0";
            if (isLocal) {
                if (File.Exists(VersionFile)) {
                    lock (_sync) {
                        using (var stream = new StreamReader(VersionFile)) {
                            content = stream.ReadToEnd();
                        }
                    }
                }
            }
            else {
                content = DownloadFrom(RemoteVersionFile);
            }

            int version;
            if (!int.TryParse(content, out version)) {
                Logger.LogMessage(string.Format("WhySharper suggestions version cannot be parsed, text={0}", content));
                return null;
            }
            return version;
        }

        private static string DownloadFrom(string source)
        {
            var content = new StringBuilder();

            try {
                using (var stream = new WebClient().OpenRead(source)) {
                    var reader = new StreamReader(stream);
                    string line = reader.ReadLine();
                    while (line != null) {
                        content.AppendLine(line);
                        line = reader.ReadLine();
                    }
                }
            }
            catch (Exception ex) {
                Logger.LogException("Failed to load remote file from " + source, ex);
            }

            return content.ToString();
        }

        private static void WriteVersion(int version)
        {
            lock (_sync) {
                EnsureFolderExists();
                using (var stream = new StreamWriter(VersionFile, false)) {
                    stream.WriteLine(version);
                }
            }
        }

        private static void EnsureFolderExists()
        {
            if (!Directory.Exists(Folder)) {
                Directory.CreateDirectory(Folder);
            }
        }
    }
}
