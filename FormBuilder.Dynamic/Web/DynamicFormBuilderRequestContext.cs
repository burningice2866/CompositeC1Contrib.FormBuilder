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

        public override void SetDefaultValues()
        {
            var def = DynamicFormsFacade.GetFormByName(_model.Name);

            foreach (var field in _model.Fields)
            {
                XElement defaultValueSetter;
                if (!def.DefaultValues.TryGetValue(field.Name, out defaultValueSetter))
                {
                    continue;
                }

                var runtimeTree = FunctionFacade.BuildTree(defaultValueSetter);

                field.Value = runtimeTree.GetValue();
            }
        }

        public override void OnSubmit()
        {
            var def = DynamicFormsFacade.GetFormByName(_model.Name);

            foreach (var handler in def.SubmitHandlers)
            {
                handler.Submit(_model);
            }
        }
    }
}
