using System;
using System.IO;
using System.Xml;
using JetBrains.ComponentModel;
using JetBrains.Util;

namespace WhySharper
{
    /// <summary>
    /// Provides IXmlExternalizableComponent part of the implementation. 
    /// 
    /// Our plugin doesn't actually need anything here - just the tag name and scope, 
    /// to make things working.
    /// </summary>
	public partial class Downloader
	{
	    /// <summary>
        /// Gets the Tag Name of the XML Element in the settings file.
        /// Ne tag name should not conflict with any other plugins and internal ReSharper components.
        /// </summary>
        public string TagName
        {
            get { return "WhySharper.Downloader"; }
        }

        /// <summary>
        /// Gets the scope that defines which store the data goes into. Must not be <c>null</c>.
        /// </summary>
        public XmlExternalizationScope Scope
        {
            get { return XmlExternalizationScope.UserSettings; }
        }

        /// <summary>
        /// This method is called on the component to populate it with default or loaded settings, 
        /// unless the component implements <see cref="T:JetBrains.Util.IXmlUpgradable"/> and 
        /// returns <c>true</c> from its handler.
        /// 
        /// For the first time, this method is called right before component's init. The settings-reading 
        /// protocol might be executed more than once thru the component lifetime.
        ///  </summary>
        /// <param name="element">The element is taken from the settings file, if available.
        /// If not, and there are settings files from the older versions, and the component does 
        /// not implement <see cref="T:JetBrains.Util.IXmlUpgradable"/>, this method is called with 
        /// the old settings. Otherwise, an empty element or <c>null</c> is passed.</param>
        public void ReadFromXml(XmlElement element)
        {
            //we do nothing here
        }

        /// <summary>
        /// Called when the component should serialize its settings into the XML presentation, 
        /// for saving into the settings file.
        /// </summary>
        public void WriteToXml(XmlElement element)
        {
            // we do nothing here
        }
	}
}
