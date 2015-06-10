using System;
using System.Linq;
using System.Web;
using System.Xml.Linq;

using Composite.AspNet.Razor;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Functions;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Validation;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public abstract class FormsPage : RazorFunction
    {
        protected abstract FormBuilderRequestContext RenderingContext { get; }

        [FunctionParameter(Label = "Intro text", DefaultValue = null)]
        public XhtmlDocument IntroText { get; set; }

        [FunctionParameter(Label = "Success response", DefaultValue = null)]
        public XhtmlDocument SuccessResponse { get; set; }

        public FormOptions Options
        {
            get { return RenderingContext.Options; }
        }

        public FormModel RenderingModel
        {
            get { return RenderingContext.RenderingModel; }
        }

        protected bool IsOwnSubmit
        {
            get { return RenderingContext.IsOwnSubmit; }
        }

        protected bool IsSuccess
        {
            get { return RenderingContext.IsSuccess; }
        }

        public ValidationResultList ValidationResult
        {
            get { return RenderingContext.ValidationResult; }
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

        public override void ExecutePageHierarchy()
        {
            if (RenderingContext.IsSuccess)
            {
                HandleSubmit();
            }

            base.ExecutePageHierarchy();
        }

        public virtual void HandleSubmit()
        {
            RenderingContext.Submit();
        }

        protected IHtmlString WriteErrors()
        {
            return RenderingContext.IsOwnSubmit ? FormRenderer.WriteErrors(ValidationResult, Options) : new HtmlString(String.Empty);
        }

        protected IHtmlString WriteAllFields()
        {
            return RenderModelFields(RenderingModel, ValidationResult, Options);
        }

        public static IHtmlString RenderModelFields(IFormModel model, ValidationResultList validationResult, FormOptions options)
        {
            var renderingMarkup = RenderingLayoutFacade.GetRenderingLayout(model.Name);

            foreach (var field in model.Fields.Where(f => f.Label != null))
            {
                var fieldElement = renderingMarkup.Body.Descendants().SingleOrDefault(el => el.Name == Namespaces.Xhtml + "p" && el.Value.Trim() == "%" + field.Name + "%");
                if (fieldElement == null)
                {
                    continue;
                }

                var textBefore = field.Attributes.OfType<TextBeforeAttribute>().SingleOrDefault();
                if (textBefore != null)
                {
                    var text = textBefore.Text;
                    if (!text.StartsWith("<"))
                    {
                        text = "<p>" + text + "</p>";
                    }

                    fieldElement.AddBeforeSelf(XElement.Parse(text));
                }
                
                var html = FormRenderer.FieldFor(options, field, validationResult).ToString();
                var newValue = XElement.Parse(html);

                fieldElement.ReplaceWith(newValue);
            }

            return new HtmlString(renderingMarkup.Body.ToString());
        }

        protected IHtmlString WriteCaptcha()
        {
            return FormRenderer.Captcha(RenderingContext);
        }

        protected bool HasDependencyChecks()
        {
            return RenderingContext.RenderingModel.Fields.Select(f => f.DependencyAttributes).Any();
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
            return BeginRow(false);
        }

        protected FieldsRow BeginRow(bool includeLabels)
        {
            return new FieldsRow(this, includeLabels);
        }

        protected string WriteErrorClass(string name, ValidationResultList validationResult, FormOptions options)
        {
            return FormRenderer.WriteErrorClass(name, validationResult, options);
        }
    }
}
