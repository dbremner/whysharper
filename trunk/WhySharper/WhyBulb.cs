using System;
using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Intentions;
using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
using JetBrains.ReSharper.Intentions.CSharp.ContextActions.Util;
using JetBrains.TextControl;
using JetBrains.UI.PopupMenu;

namespace WhySharper
{
    [ContextAction(Description = "Provides an easy way to find explanations to R# highlightings in the current file.", Name = "Why Resharper suggests", Priority = 0, Group = "C#")]
    public class WhyBulb : CSharpContextActionBase, IBulbItem
    {
        /// <summary>
        /// Creates a new <see cref="WhyBulb"/>.
        /// </summary>
        /// <param name="provider">Provides the context for this action: details 
        /// about the project file, document, etc.</param>
        public WhyBulb(ICSharpContextActionDataProvider provider) : base(provider) { }

        /// <summary>
        /// Returns list of bilb items available for this context action. 
        /// As far as <see cref="WhyBulb"/> is both context action and bulb item,
        /// we return itself.
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
            get { return "Why?"; }
        }

        /// <summary>
        /// The value returned from this method is analysed inside R# and, if it's true,
        /// the corresponding action gets added to the list of actions available at
        /// the current caret position.
        /// </summary>
        /// <returns>We always return true - TBH, I'd better off returning true only 
        /// if the given document contains any highlightings, but I haven't found 
        /// a QUICK way to do that.</returns>
        protected override bool IsAvailableInternal()
        {
            return true;
        }

        /// <summary>
        /// Executes the context action.
        /// </summary>
        /// <param name="param">Context parameters.</param>
        protected override void ExecuteInternal(params object[] param)
        {
            try {
                var myDeamon = new WhySharperDaemon(Provider.ProjectFile);
                myDeamon.DoHighlighting();

                Popup.Display("Why R# suggests", myDeamon.Highlightings);
            }
            catch (Exception ex) {
                var menuItems = new List<SimpleMenuItem>(1) { new SimpleMenuItem { Text = ex.ToString() } };
                Popup.Display("exception", menuItems);
            }
        }
    }
}
