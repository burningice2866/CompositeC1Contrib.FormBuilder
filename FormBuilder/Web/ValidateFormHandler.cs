using System;
using System.Linq;
using System.Text;
using System.Web;

namespace CompositeC1Contrib.FormBuilder.Web
{
    public class ValidateFormHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext ctx)
        {
            string cmd = ctx.Request.QueryString["cmd"];

            var sb = new StringBuilder();

            var formType = Type.GetType(ctx.Request.QueryString["formType"]);
            var form = (BaseForm)Activator.CreateInstance(formType, ctx.Request.Form);

            var validationResult = form.Validate().ToList();

            if (ctx.Request.AcceptTypes.Contains("application/json"))
            {
                sb.Append("{ \"errors\": [");

                foreach (var el in validationResult)
                {
                    sb.Append("{ \"affectedKeys\": [" + String.Join(",", el.AffectedFormIds.Select(id => "\"" + id + "\"")) + "], \"validationMessage\": \"" + el.ValidationMessage + "\" }");

                    if (validationResult.IndexOf(el) < (validationResult.Count - 1))
                    {
                        sb.Append(",");
                    }
                }

                sb.Append("] }");
            }
            else
            {
                sb.Append("Following fields did not validate: <br />");

                foreach (var el in validationResult)
                {
                    foreach (var key in el.AffectedFormIds)
                    {
                        sb.Append(key + "<br />");
                    }
                }
            }

            ctx.Response.Write(sb.ToString());
        }
    }
}