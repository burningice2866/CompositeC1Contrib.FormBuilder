using System;
using System.Web;
using System.Xml.Linq;

using Composite.AspNet.Razor;
using Composite.Core.WebClient.Renderings.Page;

using CompositeC1Contrib.FormBuilder.FunctionProviders;
using CompositeC1Contrib.FormBuilder.Wizard;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public abstract class StandardFormWizardPage : RazorFunction
    {
        protected FormWizard Wizard
        {
            get { return RenderingContext.Wizard; }
        }

        protected bool IsSuccess
        {
            get { return RenderingContext.IsSuccess; }
        }

        protected FormWizardRequestContext RenderingContext
        {
            get { return (FormWizardRequestContext)FunctionContextContainer.GetParameterValue(BaseFormFunction.RenderingContextKey, typeof(FormWizardRequestContext)); }
        }

        public override void ExecutePageHierarchy()
        {
            if (RenderingContext.IsSuccess)
            {
                RenderingContext.Submit();
            }

            base.ExecutePageHierarchy();
        }

        public IHtmlString EvaluateMarkup(XElement element)
        {
            if (element == null)
            {
                return null;
            }

            var doc = new XElement(element);

            PageRenderer.ExecuteEmbeddedFunctions(doc, FunctionContextContainer);

            return new HtmlString(doc.ToString());
        }

        protected IHtmlString WriteErrors()
        {
            return RenderingContext.IsOwnSubmit ? FormRenderer.WriteErrors(RenderingContext.ValidationResult) : new HtmlString(String.Empty);
        }

        protected WizardHtmlForm BeginForm()
        {
            return BeginForm(null);
        }

        protected WizardHtmlForm BeginForm(object htmlAttributes)
        {
            return new WizardHtmlForm(this, RenderingContext.RenderingModel, htmlAttributes);
        }

        protected IHtmlString RenderFormField(FormWizardStep step)
        {
            var options = new FormOptions();
            
            var model = Wizard.StepModels[step.Name];
            var html = FormsPage.RenderModelFields(model, options).ToString();
            var xelement = XElement.Parse(html);

            foreach (var element in xelement.Descendants())
            {
                var nameAttr = element.Attribute("name");
                if (nameAttr != null)
                {
                    nameAttr.Value = String.Format("step-{0}-{1}", step, nameAttr.Value);
                }
            }

            return new HtmlString(xelement.ToString());
        }
    }
}
