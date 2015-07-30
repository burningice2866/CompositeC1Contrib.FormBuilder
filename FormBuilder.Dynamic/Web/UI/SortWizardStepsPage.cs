using System;
using System.Linq;
using System.Web;
using System.Web.Services;

using CompositeC1Contrib.Sorting.Web.UI;

namespace CompositeC1Contrib.FormBuilder.Dynamic.Web.UI
{
    public class SortWizardStepsPage : BaseSortPage
    {
        [WebMethod]
        public static void UpdateOrder(string wizardName, string consoleId, string entityToken, string serializedOrder)
        {
            var def = DynamicWizardsFacade.GetWizard(wizardName);

            UpdateOrder(def, serializedOrder);

            var serializedEntityToken = HttpUtility.UrlDecode(entityToken);
            if (!String.IsNullOrEmpty(serializedEntityToken))
            {
                UpdateParents(serializedEntityToken, consoleId);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            var wizardName = Request.QueryString["wizardName"];
            var def = DynamicWizardsFacade.GetWizard(wizardName);

            Master.CustomJsonDataName = "wizardName";
            Master.CustomJsonDataValue = Request.QueryString["wizardName"];

            Master.SortableItems = def.Model.Steps.Select(i => new SortableItem
            {
                Id = i.Name,
                Name = i.Name
            });

            base.OnLoad(e);
        }

        private static void UpdateOrder(DynamicWizardDefinition def, string serializedOrder)
        {
            var newOrder = ParseNewOrder(serializedOrder);
            var tmpList = newOrder.OrderBy(i => i.Value).Select(itm => def.Model.Steps.Single(f => f.Name == itm.Key)).ToList();

            def.Model.Steps.Clear();

            foreach (var s in tmpList)
            {
                def.Model.Steps.Add(s);
            }

            DynamicWizardsFacade.SaveWizard(def);
        }
    }
}
