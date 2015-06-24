using System.Collections.Generic;
using System.Xml.Serialization;

namespace XComponent.Common.UI.Docking
{
    [XmlRoot("DockingLayoutModel")]
    public class XCDockingLayout
    {
        public XCDockingLayout()
        {
            LayoutVersion = XCDocking.DefaultLayoutVersion;
        }

        public int LayoutVersion { get; set; }

        [XmlArray("ViewModels")]
        public List<ViewModelSavedParameters> ViewModelSavedParameters { get; set; }
    }
}
