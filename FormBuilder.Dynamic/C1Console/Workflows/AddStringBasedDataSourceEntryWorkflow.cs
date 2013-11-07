using System;
using System.Linq;
using System.Workflow.Activities;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Workflows
{
    public class AddStringBasedDataSourceEntryWorkflow : Basic1StepAddDialogWorkflow
    {
        public override string FormDefinitionFileName
        {
            get { return "\\InstalledPackages\\CompositeC1Contrib.FormBuilder.Dynamic\\AddStringBasedDataSourceEntryWorkflow.xml"; }
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (!BindingExist("EntryValue"))
            {
                var dataSourceToken = (DataSourceEntityToken)EntityToken;

                Bindings.Add("EntryValue", String.Empty);
            }
        }

        public override void OnSave(object sender, EventArgs e)
        {
            var dataSourceToken = (DataSourceEntityToken)EntityToken;

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

        public override void OnValidate(object sender, ConditionalEventArgs e)
        {
            var dataSourceToken = (DataSourceEntityToken)EntityToken;

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
    }
}
