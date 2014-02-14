using System;
using System.Linq;
using System.Web;
using System.Xml.Linq;

using Composite.AspNet.Razor;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Functions;

using CompositeC1Contrib.FormBuilder.Attributes;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public abstract class FormsPage : RazorFunction
    {
        protected FormOptions Options { get; private set; }
        protected abstract FormBuilderRequestContext RenderingContext { get; }

        [FunctionParameter(Label = "Intro text", DefaultValue = null)]
        public XhtmlDocument IntroText { get; set; }

        [FunctionParameter(Label = "Success response", DefaultValue = null)]
        public XhtmlDocument SuccessResponse { get; set; }

        protected FormModel RenderingModel
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

        protected FormsPage()
        {
            Options = new FormOptions();
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
            if (RenderingContext.IsOwnSubmit && RenderingContext.IsSuccess)
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

        protected IHtmlString WriteErrors()
        {
            if (RenderingContext.IsOwnSubmit)
            {
                return FormRenderer.WriteErrors(RenderingContext.RenderingModel);
            }

            return new HtmlString(String.Empty);
        }

        protected IHtmlString WriteAllFields()
        {
            var model = RenderingContext.RenderingModel;
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

                var newValue = XElement.Parse(FormRenderer.FieldFor(field).ToString());

                fieldElement.ReplaceWith(newValue);
            }

            return new HtmlString(renderingMarkup.Body.ToString());
        }

        protected IHtmlString WriteCaptcha()
        {
            return FormRenderer.Captcha(RenderingModel);
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
            return new HtmlForm(this, RenderingContext.RenderingModel, htmlAttributes);
        }

        protected string WriteErrorClass(string name)
        {
            var validationResult = RenderingContext.RenderingModel.ValidationResult;

            return FormRenderer.WriteErrorClass(name, validationResult);
        }
    }
}
