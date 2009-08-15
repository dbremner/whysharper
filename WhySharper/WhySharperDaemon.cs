using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.Impl;

namespace WhySharper
{
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

        public void DoHighlighting()
        {
            DoHighlighting(DaemonProcessKind.VISIBLE_DOCUMENT, Commiter);
        }

        private void Commiter(DaemonCommitContext context)
        {
            _highlightings.AddRange(context.HighlightingsToAdd);
        }
    }
}
