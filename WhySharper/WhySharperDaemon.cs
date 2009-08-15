using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Impl;

namespace WhySharper
{
    /// <summary>
    /// Analyses the currently opened file - finds all R# errors, warnings and suggestions.
    /// </summary>
    public class WhySharperDaemon : DaemonProcessBase
    {
        public WhySharperDaemon(IProjectFile projectFile) : base(projectFile) { }
        public WhySharperDaemon(IProjectFile projectFile, IList<IDaemonStage> stages) : base(projectFile, stages) { }
        public override bool IsRangeInvalidated(DocumentRange range) { return false; }
        public override bool FullRehighlightingRequired { get { return true; } }

        private readonly List<HighlightingInfo> _highlightings = new List<HighlightingInfo>();
        public List<HighlightingInfo> Highlightings
        {
            get { return _highlightings; }
        }

        /// <summary>
        /// Performs highlighting of the currently opened document.
        /// </summary>
        public void DoHighlighting()
        {
            DoHighlighting(DaemonProcessKind.VISIBLE_DOCUMENT, Commiter);
        }

        /// <summary>
        /// Adds all highlightings to a list that will be used to display the "Why?" popup.
        /// </summary>
        /// <param name="context"></param>
        private void Commiter(DaemonCommitContext context)
        {
            _highlightings.AddRange(context.HighlightingsToAdd);
        }
    }
}
