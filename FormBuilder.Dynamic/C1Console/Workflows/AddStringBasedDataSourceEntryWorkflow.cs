using System;
using System.Linq;
using System.Workflow.Activities;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public sealed partial class AddStringBasedDataSourceEntryWorkflow : Composite.C1Console.Workflow.Activities.FormsWorkflow
    {
        public AddStringBasedDataSourceEntryWorkflow()
        {
            InitializeComponent();
        }

        private void validateSave(object sender, ConditionalEventArgs e)
        {
            var dataSourceToken = (FormFieldDataSourceEntityToken)EntityToken;

            var entryValue = GetBinding<string>("EntryValue");

            var definition = DynamicFormsFacade.GetFormByName(dataSourceToken.FormName);
            var field = definition.Model.Fields.Single(f => f.Name == dataSourceToken.FieldName);
            var datasSource = field.DataSource.Select(itm => itm.Key).ToList();

            if (datasSource.Any(itm => itm == entryValue))
            {
                ShowFieldMessage("Entry value", "Entry value is not unique");

                e.Result = false;

                return;
            }

            e.Result = true;
        }

        private void initCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            if (!BindingExist("EntryValue"))
            {
                var dataSourceToken = (FormFieldDataSourceEntityToken)EntityToken;

                Bindings.Add("EntryValue", String.Empty);
            }
        }

        private void saveCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            var dataSourceToken = (FormFieldDataSourceEntityToken)EntityToken;

            var entryValue = GetBinding<string>("EntryValue");

            var definition = DynamicFormsFacade.GetFormByName(dataSourceToken.FormName);
            var field = definition.Model.Fields.Single(f => f.Name == dataSourceToken.FieldName);
            var datasSource = field.DataSource.Select(itm => itm.Key).ToList();

            datasSource.Add(entryValue);

            var dataSourceAttribute = field.Attributes.OfType<StringBasedDataSourceAttribute>().First();

            field.Attributes.Remove(dataSourceAttribute);

            dataSourceAttribute = new StringBasedDataSourceAttribute(datasSource.ToArray());

            field.Attributes.Add(dataSourceAttribute);

            DynamicFormsFacade.SaveForm(definition);

            var treeRefresher = CreateAddNewTreeRefresher(EntityToken);
            treeRefresher.PostRefreshMesseges(new StringBasedDataSourceEntryEntityToken(dataSourceToken.FormName, dataSourceToken.FieldName, entryValue));
        }
    }
}
