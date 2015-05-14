using System;
using System.Text;
using System.Web;
using System.Xml.Linq;

using Composite.AspNet.Razor;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Functions;

using CompositeC1Contrib.FormBuilder.FunctionProviders;

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

        [FunctionParameter(Label = "Intro text", DefaultValue = null)]
        public XhtmlDocument IntroText { get; set; }

        [FunctionParameter(Label = "Success response", DefaultValue = null)]
        public XhtmlDocument SuccessResponse { get; set; }

        public override void ExecutePageHierarchy()
        {
            if (RenderingContext.IsSuccess)
            {
                HandleSubmit();
            }

            base.ExecutePageHierarchy();
        }

        public override void Execute() { }

        public virtual void HandleSubmit()
        {
            RenderingContext.Submit();
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

        protected IHtmlString WriteSteps()
        {
            var steps = Wizard.Steps;
            var sb = new StringBuilder();

            for (int i = 0; i < steps.Count; i++)
            {
                var step = steps[i];
                var stepNumber = (i + 1);

                var previousButtonLabel = step.PreviousButtonLabel;
                if (String.IsNullOrEmpty(previousButtonLabel))
                {
                    previousButtonLabel = "Tilbage";
                }

                var nextButtonLabel = step.NextButtonLabel;
                if (String.IsNullOrEmpty(nextButtonLabel))
                {
                    nextButtonLabel = "Næste";

                    if (i == (steps.Count - 1))
                    {
                        nextButtonLabel = "Indsend";
                    }
                }

                sb.AppendFormat("<div data-step=\"{0}\" class=\"js-formwizard-step step {0}\"", stepNumber);

                if (i > 0)
                {
                    sb.Append(" style=\"display: none;\"");
                }

                sb.Append(">");

                sb.Append(RenderFormField(step));

                sb.Append("<div class=\"control-group submit-buttons\"><div class=\"controls\">");

                if (i > 0)
                {
                    sb.AppendFormat("<button class=\"btn btn-primary btn-prev\" data-nextstep=\"{0}\">{1}</button>", stepNumber - 1, previousButtonLabel);
                }

                if (i < (steps.Count - 1))
                {
                    sb.AppendFormat("<button class=\"btn btn-primary btn-next\" data-nextstep=\"{0}\">{1}</button>", stepNumber + 1, nextButtonLabel);
                }

                if (i == (steps.Count - 1))
                {
                    sb.AppendFormat("<input class=\"btn btn-primary\" type=\"submit\" value=\"{0}\" />", nextButtonLabel);
                }

                sb.Append("</div></div></div>");
            }

            return new HtmlString(sb.ToString());
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

            var model = step.FormModel;
            var html = FormsPage.RenderModelFields(model, RenderingContext.ValidationResult, options).ToString();
            var xelement = XElement.Parse(html);

            foreach (var element in xelement.Descendants())
            {
                var nameAttr = element.Attribute("name");
                if (nameAttr != null)
                {
                    var stepIndex = Wizard.Steps.IndexOf(step) + 1;

                    nameAttr.Value = String.Format("step-{0}-{1}", stepIndex, nameAttr.Value);
                }
            }

            return new HtmlString(xelement.ToString());
        }
    }
}
