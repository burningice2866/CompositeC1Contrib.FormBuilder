using System.Xml;

namespace CompositeC1Contrib.FormBuilder.Configuration
{
    public interface IPluginConfiguration
    {
        void Load(XmlNode element);
    }
}
