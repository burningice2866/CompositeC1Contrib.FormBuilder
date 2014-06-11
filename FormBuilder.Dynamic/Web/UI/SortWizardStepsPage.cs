using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

using Composite.C1Console.Events;
using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.Dynamic.Web.UI
{
    public class SortWizardStepsPage : Page
    {
        protected Repeater rptFields;

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

            rptFields.DataSource = def.Steps;
            rptFields.DataBind();

            base.OnLoad(e);
        }

        private static void UpdateParents(string seralizedEntityToken, string consoleId)
        {
            var entityToken = EntityTokenSerializer.Deserialize(seralizedEntityToken);
            var graph = new RelationshipGraph(entityToken, RelationshipGraphSearchOption.Both);

            if (graph.Levels.Count() <= 1)
            {
                return;
            }

            var level = graph.Levels.ElementAt(1);
            foreach (var token in level.AllEntities)
            {
                var consoleMessageQueueItem = new RefreshTreeMessageQueueItem
                {
                    EntityToken = token
                };

                ConsoleMessageQueueFacade.Enqueue(consoleMessageQueueItem, consoleId);
            }
        }

        private static void UpdateOrder(DynamicFormWizard def, string serializedOrder)
        {
            var newOrder = new Dictionary<FormWizardStep, int>();

            serializedOrder = serializedOrder.Replace("instance[]=", ",").Replace("&", "");
            var split = serializedOrder.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < split.Length; i++)
            {
                var name = split[i];
                var step = def.Steps.Single(f => f.Name == name);

                newOrder.Add(step, i);
            }

            def.Steps.Clear();

            foreach (var itm in newOrder.OrderBy(i => i.Value))
            {
                def.Steps.Add(itm.Key);
            }

            DynamicFormWizardsFacade.SaveWizard(def);
        }
    }
}
