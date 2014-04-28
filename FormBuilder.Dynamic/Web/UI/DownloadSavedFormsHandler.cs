using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace CompositeC1Contrib.FormBuilder.Dynamic.Web.UI
{
    public class DownloadSavedFormsHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var form = context.Request.QueryString["form"];
            var formDefintion = DynamicFormsFacade.GetFormByName(form);
            var model = formDefintion.Model;
            var submits = SaveFormSubmitFacade.LoadSubmits(FormModelsFacade.FormsPath, model);

            var response = context.Response;

            var table = new XElement("table");
            var rows = new List<Dictionary<string, string>>();

            foreach (var submit in submits)
            {
                var cols = new Dictionary<string, string>();

                foreach (var field in submit.Descendants("field"))
                {
                    var key = field.Attribute("name").Value;
                    var value = field.Attribute("value").Value;

                    cols.Add(key, value);
                }

                rows.Add(cols);
            }

            var distinctFieldNames = rows.SelectMany(c => c.Keys).Distinct();

            table.Add(new XElement("tr",
                distinctFieldNames.Select(k => new XElement("th", k)))
            );

            table.Add(rows.Select(row => new XElement("tr",
                        row.Values.Select(v => new XElement("td", String.IsNullOrEmpty(v) ? String.Empty : v))
                    )
                )
            );

            response.Clear();

            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("content-disposition", String.Format("attachment;filename={0}.xls", form));
            response.Write(table.ToString());
        }
    }
}
