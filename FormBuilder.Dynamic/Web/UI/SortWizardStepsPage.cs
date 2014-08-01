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
            var def = DynamicFormWizardsFacade.GetWizard(wizardName);

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
            var def = DynamicFormWizardsFacade.GetWizard(wizardName);

            Master.CustomJsonDataName = "wizardName";
            Master.CustomJsonDataValue = Request.QueryString["wizardName"];

            Master.SortableItems = def.Steps.Select(i => new SortableItem
            {
                Id = i.Name,
                Name = i.Name
            });

            base.OnLoad(e);
        }

        private static void UpdateOrder(DynamicFormWizard def, string serializedOrder)
        {
            var newOrder = ParseNewOrder(serializedOrder);
            var tmpList = newOrder.OrderBy(i => i.Value).Select(itm => def.Steps.Single(f => f.Name == itm.Key)).ToList();

            def.Steps.Clear();

            foreach (var s in tmpList)
            {
                def.Steps.Add(s);
            }

            DynamicFormWizardsFacade.SaveWizard(def);
        }
    }
}
