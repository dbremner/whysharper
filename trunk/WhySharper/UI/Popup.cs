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
using System.Diagnostics;
using JetBrains.CommonControls;
using JetBrains.UI.PopupMenu;

namespace WhySharper.UI
{
    /// <summary>
    /// Displays WhySharper popup messages.
    /// </summary>
    internal static class Popup
    {
        /// <summary>
        /// Displays a simple "ask for a suggestion" popup, the idea is that users would submit bugs
        /// if they want to add a suggestion or more discussions to the existing suggestions. Then,
        /// WhySharper contributors would update the xml that gets updated regulary (eg, on each VS startup).
        /// </summary>
        internal static void SubmitSuggestion(string resharperName) 
        {
            var menuItems = new List<SimpleMenuItem> { CreateExplanationMenuItem(resharperName) };
            Display("Ehm, no clue.", menuItems);
        }

        /// <summary>
        /// Displays all links for the given suggestion.
        /// </summary>
        /// <param name="suggestion">Suggestion to display links for.</param>
        internal static void Display(Suggestion suggestion)
        {
            var menuItems = suggestion.Links.ConvertAll(i => CreateMenuItem("\"" + i.Key + "\"", i.Value, null));
            menuItems.Add(SimpleMenuItem.CreateSeparator());
            menuItems.Add(CreateExplanationMenuItem(suggestion.ResharperName));

            Display("How about this", menuItems);
        }

        private static SimpleMenuItem CreateExplanationMenuItem(string resharperName)
        {
            string text = "...Looking for an answer? Have an explanation? Let us know! (R# name is " + resharperName + ")";
            string tooltip = string.Format("R# suggestion name is '{0}'", resharperName);

            return CreateMenuItem(text, "http://code.google.com/p/whysharper/issues/list", tooltip);
        }

        private static void Display(string caption, List<SimpleMenuItem> menuItems)
        {
            var menu = new JetPopupMenu();
            menu.Caption.Value = WindowlessControl.Create(caption);
            menu.SetItems(menuItems.ToArray());
            menu.KeyboardAcceleration.SetValue(KeyboardAccelerationFlags.Mnemonics);
            menu.Show();
        }

        private static SimpleMenuItem CreateMenuItem(string text, string urlOnClick, string tooltip) 
        {
            const int maxLength = 80;
            if (tooltip == null) {
                tooltip = (urlOnClick.Length < maxLength) ? urlOnClick : urlOnClick.Substring(0, maxLength) + "...";
            }

            var item = new SimpleMenuItem { Text = text, Style = MenuItemStyle.Enabled, Tooltip = tooltip };
            item.Clicked += delegate { Process.Start(urlOnClick); };

            return item;
        }
    }
}
