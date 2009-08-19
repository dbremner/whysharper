using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.CommonControls;
using JetBrains.UI.PopupMenu;

namespace WhySharper.Suggestions
{
    /// <summary>
    /// Displays WhySharper popup messages.
    /// </summary>
    internal static class Popup
    {
        private static readonly SimpleMenuItem _askExplanationMenuItem = 
            CreateMenuItem("...Have a worthwhile explantion? Let us know!", "http://code.google.com/p/whysharper/issues/list");

        /// <summary>
        /// Displays a simple "ask for a suggestion" popup, the idea is that users would submit bugs
        /// if they want to add a suggestion or more discussions to the existing suggestions. Then,
        /// WhySharper contributors would update the xml that gets updated regulary (eg, on each VS startup).
        /// </summary>
        internal static void SubmitSuggestion() 
        {
            var menuItems = new List<SimpleMenuItem> { _askExplanationMenuItem };
            Display("Ehm, no clue.", menuItems);
        }

        /// <summary>
        /// Displays all links for the given suggestion.
        /// </summary>
        /// <param name="suggestion">Suggestion to display links for.</param>
        internal static void Display(Suggestion suggestion)
        {
            var menuItems = suggestion.Links.ConvertAll(i => CreateMenuItem("\"" + i.Key + "\"", i.Value));
            menuItems.Add(SimpleMenuItem.CreateSeparator());
            menuItems.Add(_askExplanationMenuItem);

            Display("How about this", menuItems);
        }

        private static void Display(string caption, List<SimpleMenuItem> menuItems)
        {
            var menu = new JetPopupMenu();
            menu.Caption.Value = WindowlessControl.Create(caption);
            menu.SetItems(menuItems.ToArray());
            menu.KeyboardAcceleration.SetValue(KeyboardAccelerationFlags.Mnemonics);
            menu.Show();
        }

        private static SimpleMenuItem CreateMenuItem(string text, string urlOnClick) 
        {
            const int maxLength = 80;
            string tooltip = (urlOnClick.Length < maxLength) ? urlOnClick : urlOnClick.Substring(0, maxLength) + "...";

            var item = new SimpleMenuItem { Text = text, Style = MenuItemStyle.Enabled, Tooltip = tooltip };
            item.Clicked += delegate { Process.Start(urlOnClick); };

            return item;
        }
    }
}