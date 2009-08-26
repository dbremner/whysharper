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
using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.Util;
using WhySharper.UI;

namespace WhySharper
{
    /// <summary>
    /// Adds a <see cref="WhyBulb"/> of "disable highlighting" type for every given highlighting. 
    /// </summary>
    [DisableHighlightingActionProvider]
    public class SuggestionProvider : IDisableHighlightingActionProvider
    {
        /// <summary>
        /// Returns a WhyBulb for the given highlighting.
        /// </summary>
        /// <param name="highlighting">Highlighting to process.</param>
        /// <param name="highlightingRange">Hihglighting range - not used here.</param>
        /// <returns></returns>
        public ICollection<IDisableHighlightingAction> Actions(IHighlighting highlighting, DocumentRange highlightingRange)
        {
            var cSharpHighlighting = highlighting as CSharpHighlightingBase;
            return (cSharpHighlighting == null) 
                       ? EmptyArray<IDisableHighlightingAction>.Instance 
                       : new IDisableHighlightingAction[] { new WhyBulb(highlighting.ErrorStripeToolTip, highlighting.GetType().Name) };
        }
    }
}