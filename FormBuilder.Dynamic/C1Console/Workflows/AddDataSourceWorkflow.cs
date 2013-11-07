using System;
using System.Collections.Generic;
using System.Linq;
using System.Workflow.Activities;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class AddDataSourceWorkflow : Basic1StepAddDialogWorkflow
    {
        public override string FormDefinitionFileName
        {
            get { return "\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\AddDataSourceWorkflow.xml"; }
        }

        public static Dictionary<string, string> GetDataSourceTypes()
        {
            return new Dictionary<string, string>
            {
                { typeof(StringBasedDataSourceAttribute).AssemblyQualifiedName, "Stringbased datasource" }
            };
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (!BindingExist("DataSourceType"))
            {
                Bindings.Add("DataSourceType", String.Empty);
            }
        }

        public override void OnSave(object sender, EventArgs e)
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
