using System;
using System.Linq;
using System.Web;
using System.Web.Services;

using CompositeC1Contrib.Sorting.Web.UI;

namespace CompositeC1Contrib.FormBuilder.Dynamic.Web.UI
{
    public class SortFormFieldsPage : BaseSortPage
    {
        [WebMethod]
        public static void UpdateOrder(string formName, string consoleId, string entityToken, string serializedOrder)
        {
            var formDefinition = DynamicFormsFacade.GetFormByName(formName);

            UpdateOrder(formDefinition, serializedOrder);

            var serializedEntityToken = HttpUtility.UrlDecode(entityToken);
            if (!String.IsNullOrEmpty(serializedEntityToken))
            {
                UpdateParents(serializedEntityToken, consoleId);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            var formName = Request.QueryString["formName"];

            if (Request.HttpMethod == "POST")
            {
                formName = Request.Form["formName"];
            }

            var formDefinition = DynamicFormsFacade.GetFormByName(formName);

            Master.CustomJsonDataName = "formName";
            Master.CustomJsonDataValue = formName;

            Master.SortableItems = formDefinition.Model.Fields.Select(i => new SortableItem
            {
                Id = i.Name,
                Name = i.Name
            });

            base.OnLoad(e);
        }

        private static void UpdateOrder(DynamicFormDefinition formDefinition, string serializedOrder)
        {
            var newOrder = ParseNewOrder(serializedOrder);
            var tmpList = newOrder.OrderBy(i => i.Value).Select(itm => formDefinition.Model.Fields.Single(f => f.Name == itm.Key)).ToList();

            formDefinition.Model.Fields.Clear();

            foreach (var f in tmpList)
            {
                formDefinition.Model.Fields.Add(f);
            }

            DynamicFormsFacade.SaveForm(formDefinition);
        }
    }
}
