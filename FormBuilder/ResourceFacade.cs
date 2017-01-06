using System.Globalization;
using System.Resources;

using CompositeC1Contrib.Localization;

namespace CompositeC1Contrib.FormBuilder
{
    public static class ResourceFacade
    {
        public static readonly ResourceManager InternalResourceManager = new ResourceManager("CompositeC1Contrib.FormBuilder.Strings", typeof(ResourceFacade).Assembly);

        public static IResourceWriter GetResourceWriter()
        {
            return GetResourceWriter(CultureInfo.CurrentCulture);
        }

        public static IResourceWriter GetResourceWriter(CultureInfo culture)
        {
            return new C1ResourceWriter("FormBuilder", culture);
        }
    }
}
