using System.Linq;
using System.Web;
using System.Xml.Linq;

using Composite.AspNet.Razor;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Functions;

using CompositeC1Contrib.FormBuilder.Validation;
using CompositeC1Contrib.FormBuilder.Web.UI.Rendering;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public abstract class FormsPage : RazorFunction
    {
        protected abstract FormRequestContext RequestContext { get; }

        [FunctionParameter(Label = "Intro text", DefaultValue = null)]
        public XhtmlDocument IntroText { get; set; }

        [FunctionParameter(Label = "Success response", DefaultValue = null)]
        public XhtmlDocument SuccessResponse { get; set; }

        public FormRenderer FormRenderer
        {
            get { return RequestContext.FormRenderer; }
        }

        public Form Form => RequestContext.ModelInstance;

        protected bool IsOwnSubmit => RequestContext.IsOwnSubmit;

        protected bool IsSuccess => RequestContext.IsSuccess;

        public ValidationResultList ValidationResult => RequestContext.ValidationResult;

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

        public override void ExecutePageHierarchy()
        {
            if (RequestContext.IsSuccess)
            {
                HandleSubmit();
            }


            if (IntroText == null)
            {
                var value = Localization.EvaluateT(Form.Model, "IntroText", null);
                if (value != null)
                {
                    try
                    {
                        IntroText = XhtmlDocument.Parse(value);
                    }
                    catch { }
                }
            }

            if (SuccessResponse == null)
            {
                var value = Localization.EvaluateT(Form.Model, "SuccessResponse", null);
                if (value != null)
                {
                    try
                    {
                        SuccessResponse = XhtmlDocument.Parse(value);
                    }
                    catch { }
                }
            }

            base.ExecutePageHierarchy();
        }

        public virtual void HandleSubmit()
        {
            RequestContext.Submit();
        }

        protected IHtmlString ValidationSummary()
        {
            return FormRenderer.ValidationSummary(RequestContext);
        }

        protected IHtmlString WriteAllFields()
        {
            return RenderModelFields(Form, RequestContext);
        }

        public static IHtmlString RenderModelFields(IModelInstance instance, BaseFormBuilderRequestContext context)
        {
            var renderingMarkup = RenderingLayoutFacade.GetRenderingLayout(instance.Name);

            foreach (var field in instance.Fields.Where(f => f.Label != null))
            {
                var fieldElement = renderingMarkup.Body.Descendants().SingleOrDefault(el => el.Name == Namespaces.Xhtml + "p" && el.Value.Trim() == "%" + field.Name + "%");
                if (fieldElement == null)
                {
                    continue;
                }

                var html = context.FormRenderer.FieldFor(context, field).ToString();
                var newValue = XElement.Parse(html);

                fieldElement.ReplaceWith(newValue);
            }

            return new HtmlString(renderingMarkup.Body.ToString());
        }

        protected IHtmlString WriteCaptcha()
        {
            return FormRenderer.Captcha(RequestContext);
        }

        protected bool HasDependencyChecks()
        {
            return RequestContext.ModelInstance.Fields.Select(f => f.DependencyAttributes).Any();
        }

        protected HtmlForm BeginForm()
        {
            return BeginForm(null);
        }

        protected HtmlForm BeginForm(object htmlAttributes)
        {
            return new HtmlForm(this, htmlAttributes);
        }

        protected FieldsGroup BeginGroup()
        {
            return BeginGroup(null);
        }

        protected FieldsGroup BeginGroup(string extraClass)
        {
            return new FieldsGroup(this, extraClass);
        }

        protected ControlsGroup BeginControls()
        {
            return new ControlsGroup(this);
        }

        protected FieldsRow BeginRow()
        {
            return new FieldsRow(this, FormRenderer);
        }

        protected string WriteErrorClass(string name, BaseFormBuilderRequestContext context)
        {
            return FormRenderer.WriteErrorClass(name, context);
        }
    }
}
