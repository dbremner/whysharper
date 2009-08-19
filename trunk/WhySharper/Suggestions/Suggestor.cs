using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.Util;

namespace WhySharper.Suggestions
{
    /// <summary>
    /// Adds one <see cref="WhyBulb"/> of "disable highlighting" type for every given highlighting. 
    /// </summary>
    [DisableHighlightingActionProvider]
    public class Suggestor : IDisableHighlightingActionProvider
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