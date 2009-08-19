using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Intentions;
using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
using JetBrains.ReSharper.Intentions.CSharp.ContextActions.Util;
using JetBrains.TextControl;
using WhySharper.Suggestions;

namespace WhySharper
{
    [ContextAction(Description = "Provides an easy way to find explanations to R# highlightings in the current file.", Name = "Why Resharper suggests", Priority = 0, Group = "C#")]
    public class WhyBulb : CSharpContextActionBase, IBulbItem, IDisableHighlightingAction
    {
        private readonly string _whyText;
        private readonly string _resharperName;

        /// <summary>
        /// Creates a new <see cref="WhyBulb"/>. Constructor for R# - we'll use a different one.
        /// </summary>
        /// <param name="provider">Provides the context for this action: details 
        /// about the project file, document, etc.</param>
        public WhyBulb(ICSharpContextActionDataProvider provider) : base(provider) { }

        /// <summary>
        /// Used by <see cref="Suggestor"/> to create a new <see cref="WhyBulb"/> for the given highlighting.
        /// </summary>
        /// <param name="whyText">Text to be displayed in next to the bulb.</param>
        /// <param name="resharperName">R# name of the corresponding error/warning/suggestion.</param>
        internal WhyBulb(string whyText, string resharperName) : base(null)
        {
            _whyText = whyText;
            _resharperName = resharperName;
        }

        /// <summary>
        /// Returns list of bilb items available for this context action. 
        /// As far as <see cref="WhyBulb"/> is both context action and bulb item, we return itself.
        /// </summary>
        public override IBulbItem[] Items
        {
            get { return new IBulbItem[] { this }; }
        }

        /// <summary>
        /// Executes the action for any given solution and document.
        /// </summary>
        /// <param name="solution">Solution to execute the action against.</param>
        /// <param name="textControl">Text control providing details about the document, etc.</param>
        public void Execute(ISolution solution, ITextControl textControl)
        {
            ExecuteInternal(null);
        }

        /// <summary>
        /// Returns text to be displayed next to the light bulb icon for this action.
        /// </summary>
        public string Text
        {
            get { return "...Why " + _whyText  + "?"; }
        }

        /// <summary>
        /// This value gets analysed inside R# and, if it's <c>true</c>, the corresponding 
        /// action gets added to the list of actions available at the current caret position.
        /// </summary>
        /// <returns>Returns <c>true</c> for bulbs created by WhySharper.</returns>
        protected override bool IsAvailableInternal()
        {
            return (_whyText != null);
        }

        /// <summary>
        /// Executes the context action.
        /// </summary>
        /// <param name="param">Context parameters.</param>
        protected override void ExecuteInternal(params object[] param)
        {
            var suggestion = SuggestionBrowser.GetSuggestion(_resharperName);
            if (suggestion == null || suggestion.Links.Count == 0) {
                Popup.SubmitSuggestion();
                return;
            }

            Popup.Display(suggestion);
        }
    }
}
