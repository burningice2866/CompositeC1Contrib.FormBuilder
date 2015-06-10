using System;

using CompositeC1Contrib.FormBuilder.Configuration;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public class FormOptions
    {
        private static readonly IFormFormRenderer DefaultFormFormRenderer = (IFormFormRenderer)Activator.CreateInstance(FormBuilderConfiguration.GetSection().RendererImplementation);

        public bool HideLabels { get; set; }
        public IFormFormRenderer FormRenderer { get; set; }

        public FormOptions()
        {
            HideLabels = false;
            FormRenderer = DefaultFormFormRenderer;
        }
    }
}
