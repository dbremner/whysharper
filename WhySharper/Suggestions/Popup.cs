using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.CommonControls;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Daemon.CSharp.Stages;
using JetBrains.UI.PopupMenu;
using JetBrains.UI.RichText;

namespace WhySharper.Suggestions
{
    internal static class Popup
    {
        private const string SubmitBugUrl = "http://code.google.com/p/whysharper/issues/list";

        internal static void Display(string caption, List<SimpleMenuItem> menuItems)
        {
            var menu = new JetPopupMenu();
            menu.Caption.Value = WindowlessControl.Create(caption);
            menu.SetItems(menuItems.ToArray());
            menu.KeyboardAcceleration.SetValue(KeyboardAccelerationFlags.Mnemonics);
            menu.Show();
        }

        public static void Display(string caption, List<HighlightingInfo> highlightings)
        {
            var menuItems = highlightings.ConvertAll(h => CreateMenuItem(h));
            Display(caption, menuItems);
        }

        public static void Display(string caption, string singleItem)
        {
            Display(caption, singleItem, null);
        }

        private static void Display(string caption, string singleItem, EventHandler onClick)
        {
            var item = new SimpleMenuItem { Text = singleItem };
            if (onClick != null)
            {
                item.Style = MenuItemStyle.Enabled;
                item.Clicked += onClick;
            }
            Display(caption, new List<SimpleMenuItem> { item });
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
                const string text = "Do you want to create a bug so that we can find an explanation and link to it?";
                Display("No explanation to this so far.", text, delegate { Process.Start(SubmitBugUrl); });
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
            if (name == typeof(MemberCanBeMadeStaticLocalWarning).Name)
            {
                return "http://stackoverflow.com/questions/169378/c-method-can-be-made-static-but-should-it";
            }
            else if (name == typeof(ConvertToConstantLocalWarning).Name)
            {
                return "http://stackoverflow.com/questions/909681/resharper-always-suggesting-me-to-make-const-string-instead-of-string";
            }
            else if (name == typeof(UnusedUsingDirectiveWarning).Name)
            {
                return "http://stackoverflow.com/questions/235250/what-are-the-benefits-of-maintaining-a-clean-list-of-using-directives-in-c";
            }
            return string.Empty;
        }
    }
}