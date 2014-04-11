using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Xml.Linq;

using Composite.AspNet.Razor;

using CompositeC1Contrib.FormBuilder.Wizard;

namespace CompositeC1Contrib.FormBuilder.Web.UI
{
    public abstract class StandardFormWizardPage : RazorFunction
    {
        public string WizardName { get; set; }

        protected IDictionary<string, FormModel> StepModels = new Dictionary<string, FormModel>();

        protected bool IsSuccess { get; private set; }

        protected FormWizard Wizard
        {
            get
            {
                var wizard = FormWizardsFacade.GetWizard(WizardName);

                return wizard;
            }
        }

        public override void ExecutePageHierarchy()
        {
            if (IsPost)
            {
                var steps = Wizard.Steps;
                for (int i = 0; i < steps.Count; i++)
                {
                    var step = steps[i];
                    var model = FormModelsFacade.GetModel(step.FormName);
                    var stepPrepend = "step-" + (i + 1) + "-";
                    var keys = Request.Form.AllKeys.Where(k => k.StartsWith(stepPrepend)).ToArray();
                    var nmc = new NameValueCollection();
                    foreach (var key in keys)
                    {
                        var name = key.Remove(0, stepPrepend.Length);
                        var value = Request.Form[key];

                        nmc.Add(name, value);
                    }

                    model.MapValues(nmc, Enumerable.Empty<FormFile>());

                    IsSuccess = !model.ValidationResult.Any();
                    if (!IsSuccess)
                    {
                        break;
                    }

                    StepModels.Add(step.Name, model);
                }
            }

            base.ExecutePageHierarchy();
        }

        protected IHtmlString RenderFormField(int step, string formName)
        {
            var model = FormModelsFacade.GetModel(formName);
            var options = new FormOptions();

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
