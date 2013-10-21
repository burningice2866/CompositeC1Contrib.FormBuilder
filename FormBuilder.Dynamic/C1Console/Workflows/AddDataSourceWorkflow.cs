using System;
using System.Collections.Generic;
using System.Linq;
using System.Workflow.Activities;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public sealed partial class AddDataSourceWorkflow : Composite.C1Console.Workflow.Activities.FormsWorkflow
    {
        public AddDataSourceWorkflow()
        {
            InitializeComponent();
        }

        public static Dictionary<string, string> GetDataSourceTypes()
        {
            return new Dictionary<string, string>
            {
                { typeof(StringBasedDataSourceAttribute).AssemblyQualifiedName, "Stringbased datasource" }
            };
        }

        private void validateSave(object sender, ConditionalEventArgs e)
        {
            e.Result = true;
        }

        private void initCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            if (!BindingExist("DataSourceType"))
            {
                Bindings.Add("DataSourceType", String.Empty);
            }
        }

        private void saveCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            var token = (FormFieldEntityToken)EntityToken;
            var definition = DynamicFormsFacade.GetFormByName(token.FormName);
            var field = definition.Model.Fields.Single(f => f.Name == token.FieldName);

            DataSourceAttribute attribute;
            var datasourceType = Type.GetType(GetBinding<string>("DataSourceType"));

            if (datasourceType == typeof(StringBasedDataSourceAttribute))
            {
                attribute = (DataSourceAttribute)Activator.CreateInstance(datasourceType, new string[0]);
            }
            else
            {
                attribute = (DataSourceAttribute)Activator.CreateInstance(datasourceType);
            }

            field.Attributes.Add(attribute);

            DynamicFormsFacade.SaveForm(definition);

            var treeRefresher = CreateSpecificTreeRefresher();
            treeRefresher.PostRefreshMesseges(EntityToken);
        }
    }
}
