using System;
using System.Text;
using System.Web;
using System.Xml.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Composite.AspNet.Razor;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Functions;

using CompositeC1Contrib.FormBuilder.Data;
using CompositeC1Contrib.FormBuilder.FunctionProviders;
using CompositeC1Contrib.FormBuilder.Web.UI.Rendering;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public abstract class StandardFormWizardPage : RazorFunction
    {
        [FunctionParameter(Label = "Intro text", DefaultValue = null)]
        public XhtmlDocument IntroText { get; set; }

        [FunctionParameter(Label = "Success response", DefaultValue = null)]
        public XhtmlDocument SuccessResponse { get; set; }

        protected Wizard Wizard
        {
            get { return RequestContext.Wizard; }
        }

        public FormRenderer FormRenderer
        {
            get { return RequestContext.FormRenderer; }
        }

        protected bool IsSuccess
        {
            get { return RequestContext.IsSuccess; }
        }

        protected WizardRequestContext RequestContext
        {
            get { return (WizardRequestContext)FunctionContextContainer.GetParameterValue(BaseFormFunction.RequestContextKey, typeof(WizardRequestContext)); }
        }

        public override void ExecutePageHierarchy()
        {
            if (RequestContext.IsSuccess)
            {
                HandleSubmit();
            }

            base.ExecutePageHierarchy();
        }

        public override void Execute() { }

        public virtual void HandleSubmit()
        {
            RequestContext.Submit();
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

        protected IHtmlString ValidationSummary()
        {
            return FormRenderer.ValidationSummary(RequestContext);
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

                if (HasMoreSteps(i, steps.Count))
                {
                    sb.AppendFormat("<button class=\"btn btn-primary btn-next\" data-nextstep=\"{0}\">{1}</button>", stepNumber + 1, nextButtonLabel);
                }

                if (IsLastStep(i, steps.Count))
                {
                    if (Wizard.RequiresCaptcha)
                    {
                        var html = FormRenderer.Captcha(RequestContext);

                        sb.Append(html);
                    }

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
            return new WizardHtmlForm(this, RequestContext.ModelInstance, htmlAttributes);
        }

        protected IHtmlString RenderFormField(WizardStep step)
        {
            var form = step.Form;
            var html = FormsPage.RenderModelFields(form, RequestContext).ToString();
            var xelement = XElement.Parse(html);

            foreach (var element in xelement.Descendants())
            {
                var nameAttr = element.Attribute("name");
                if (nameAttr != null)
                {
                    var stepIndex = Wizard.Steps.IndexOf(step) + 1;

                    nameAttr.Value = String.Format("step-{0}-{1}", stepIndex, nameAttr.Value);
                }

                var depAttr = element.Attribute("data-dependency");
                if (depAttr != null)
                {
                    var stepIndex = Wizard.Steps.IndexOf(step) + 1;

                    var dependencies = JsonConvert.DeserializeObject<DependencyModel[]>(depAttr.Value);
                    foreach (var dep in dependencies)
                    {
                        dep.Field = String.Format("step-{0}-{1}", stepIndex, dep.Field);
                    }

                    var json = JsonConvert.SerializeObject(dependencies, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                    depAttr.Value = json;
                }
            }

            return new HtmlString(xelement.ToString());
        }

        private static bool HasMoreSteps(int ix, int count)
        {
            return ix < (count - 1);
        }

        private static bool IsLastStep(int ix, int count)
        {
            return ix == (count - 1);
        }
    }
}
