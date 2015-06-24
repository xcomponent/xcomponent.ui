using System;
using System.Resources;
using System.Windows.Markup;
using XComponent.Common.UI.Properties;

namespace XComponent.Common.UI.I18n
{
    public class TranslateExtension : MarkupExtension
    {
        readonly string key;
        private ResourceManager _resourceManager;
        readonly string param;

        public TranslateExtension(string key)
        {
            this.key = key;
        }

        public TranslateExtension(string key, string param)
        {
            this.key = key;
            this.param = param;
        }

        const string NotFoundError = "#StringNotFound#";

        public ResourceManager ResourceManager
        {
            get { return _resourceManager; }
            set { _resourceManager = value; }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_resourceManager == null)
            {
                _resourceManager = Resources.ResourceManager;
            }
            if (string.IsNullOrEmpty(key))
                return NotFoundError;
            if (_resourceManager.GetString(key) == null)
                return NotFoundError;

            if(param!=null)
            {
                return string.Format(_resourceManager.GetString(key), param);
            }
            return _resourceManager.GetString(key);
        }
    } 
}
