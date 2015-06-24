using System;
using System.Xml;

namespace XComponent.Common.UI.Docking
{
    [Serializable]
    public class ViewModelSavedParameters
    {
        public ViewModelSavedParameters()
        {
            LayoutVersion = XCDocking.DefaultLayoutVersion;
        }

        public ViewModelSavedParameters(int layoutVersion, string panelName, string panelTypeKey, string panelTitle, string customParams)
        {
            LayoutVersion = layoutVersion;
            this.PanelName = panelName;            
            this.Type = panelTypeKey;
            this.PanelTitle = panelTitle;
            this.customParameters = customParams;
        }

        public string PanelTitle { get; set; }

        public string Type { get; set; }

        public string PanelName { get; set; }

        [NonSerialized]
        private string customParameters;
        
        public string CustomParametersStr
        {
            get { return this.customParameters; }
        }

        public XmlCDataSection CustomParameters
        {
            get
            {
                var document = new XmlDocument();
                return document.CreateCDataSection(this.customParameters);
            }
            set
            {
                this.customParameters = value.Value;
            }
        }

        public int LayoutVersion { get; set; }
    }
}
