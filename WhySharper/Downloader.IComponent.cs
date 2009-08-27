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

namespace WhySharper
{
    /// <summary>
    /// Provides IComponent part of the implementation. 
    /// </summary>
    public partial class Downloader
    {
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
    }
}
