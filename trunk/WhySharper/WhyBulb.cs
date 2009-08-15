using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.CommonControls;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Intentions;
using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
using JetBrains.ReSharper.Intentions.CSharp.ContextActions.Util;
using JetBrains.TextControl;
using JetBrains.UI.PopupMenu;
using JetBrains.UI.RichText;

namespace WhySharper
{
    [ContextAction(
        Description = "Provides an easy way to find an explanation to R# suggestions/errors/warnings highlighted in the current file.", 
        Name = "Why Resharper suggests", 
        Priority = 0, 
        Group = "C#")]
    public class WhyBulb : CSharpContextActionBase, IBulbItem
    {
        private const string SubmitBugUrl = "http://code.google.com/p/whysharper/issues/list";

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

                var menuItems = myDeamon.Highlightings.ConvertAll(h => CreateMenuItem(h));
                DisplayMenu(menuItems, "Why R# suggests");
            }
            catch (Exception ex) {
                var menuItems = new List<SimpleMenuItem>(1) { new SimpleMenuItem { Text = ex.ToString() } };
                DisplayMenu(menuItems, "exception");
            }
        }

        private static void DisplayMenu(List<SimpleMenuItem> menuItems, string caption)
        {
            var menu = new JetPopupMenu();
            menu.Caption.Value = WindowlessControl.Create(caption);
            menu.SetItems(menuItems.ToArray());
            menu.KeyboardAcceleration.SetValue(KeyboardAccelerationFlags.Mnemonics);
            menu.Show();
        }

        private static SimpleMenuItem CreateMenuItem(HighlightingInfo info)
        {
            string problemName = info.Highlighting.GetType().Name;

            var result = new SimpleMenuItem {
                Text = info.Highlighting.ErrorStripeToolTip, 
                Style = MenuItemStyle.Enabled,
                Tooltip = new RichText(problemName, TextStyle.Default)
            };
            result.Clicked += delegate { RedirectToExplanation(problemName); };  

            return result;
        }

        private static void RedirectToExplanation(string problemName)
        {
            string url = GetExplanationUrl(problemName);
            if (url != string.Empty) {
                Process.Start(url);
            } 
            else {
                var menuItem = new SimpleMenuItem {Text = "Would you mind creating a bug so that we add one?"};
                menuItem.Clicked += delegate { Process.Start(SubmitBugUrl); };
                var menuItems = new List<SimpleMenuItem>(1) { menuItem };
                DisplayMenu(menuItems, "WhySharper doesn't know about an explanation to this.");
            }
        }

        /// <summary>
        /// Find the explanation URL from given R# warning type name.
        /// (Might be later put in a settings page, with an option to update.)
        /// </summary>
        /// <param name="name">Resharper warning/suggestion name.</param>
        /// <returns></returns>
        private static string GetExplanationUrl(string name)
        {
            if (name == typeof(MemberCanBeMadeStaticLocalWarning).Name) {
                return "http://stackoverflow.com/questions/169378/c-method-can-be-made-static-but-should-it";
            }
            else if (name == typeof(ConvertToConstantLocalWarning).Name) {
                return "http://stackoverflow.com/questions/909681/resharper-always-suggesting-me-to-make-const-string-instead-of-string";
            }
            else if (name == typeof(UnusedUsingDirectiveWarning).Name) {
                return "http://stackoverflow.com/questions/235250/what-are-the-benefits-of-maintaining-a-clean-list-of-using-directives-in-c";
            }
            return string.Empty;
        }
    }
}
