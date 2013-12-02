using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console
{
    public class InputElementHandler
    {
        public string Name { get; set; }
        public InputElementTypeAttribute ElementType { get; set; }
        public IInputTypeSettingsHandler SettingsHandler { get; set; }
    }
}
