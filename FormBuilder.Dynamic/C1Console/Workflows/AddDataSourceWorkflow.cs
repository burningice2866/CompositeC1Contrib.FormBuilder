using System;
using System.Collections.Generic;
using System.Linq;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.C1Console.EntityTokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class AddDataSourceWorkflow : Basic1StepDialogWorkflow
    {
        public AddDataSourceWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\AddDataSourceWorkflow.xml") { }

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

        public override void OnFinish(object sender, EventArgs e)
        {
            var token = (FormFieldEntityToken)EntityToken;
            var definition = DynamicFormsFacade.GetFormByName(token.FormName);
            var field = definition.Model.Fields.Get(token.FieldName);

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

            field.EnsureValueType();

            DynamicFormsFacade.SaveForm(definition);

            var treeRefresher = CreateSpecificTreeRefresher();
            treeRefresher.PostRefreshMesseges(EntityToken);
        }
    }
}
