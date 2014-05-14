using System.Xml.Linq;

using Composite.Functions;

using CompositeC1Contrib.FormBuilder.Dynamic;

namespace CompositeC1Contrib.FormBuilder.Web
{
    public class DynamicFormBuilderRequestContext : FormBuilderRequestContext
    {
        private readonly FormModel _model;

        public override FormModel RenderingModel
        {
            get { return _model; }
        }

        public DynamicFormBuilderRequestContext(string formName)
            : base(formName)
        {
            _model = DynamicFormsFacade.GetFormByName(formName).Model;
        }

        public override void Submit()
        {
            var def = DynamicFormsFacade.GetFormByName(_model.Name);

            foreach (var handler in def.SubmitHandlers)
            {
                handler.Submit(_model);
            }

            base.Submit();
        }
    }
}
