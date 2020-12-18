using System;
using System.IO;
using System.Net;
using System.Text;

using CompositeC1Contrib.FormBuilder.Web.Api.Models;
using CompositeC1Contrib.FormBuilder.Web.UI.Rendering;

using Newtonsoft.Json;

namespace CompositeC1Contrib.FormBuilder.Web.Api.Handlers
{
    // formbuilder/renderer/validationsummary
    public class RendererHandler : FormBuilderHttpHandlerBase
    {
        protected override void ProcessRequest()
        {
            if (!EnsureHttpMethod("POST"))
            {
                return;
            }

            var body = GetDocumentContents(Context.Request.InputStream);
            var model = JsonConvert.DeserializeObject<ValidationSummaryModel>(body);

            if (String.IsNullOrEmpty(model.Renderer))
            {
                Context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return;
            }

            var type = Type.GetType(model.Renderer);
            if (type == null || !typeof(FormRenderer).IsAssignableFrom(type))
            {
                Context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return;
            }

            var renderer = (FormRenderer)Activator.CreateInstance(type);
            var summary = renderer.ValidationSummary(model.Errors);

            body = JsonConvert.SerializeObject(summary);

            Context.Response.StatusCode = (int)HttpStatusCode.OK;
            Context.Response.Write(body);
        }

        private static string GetDocumentContents(Stream inputStream)
        {
            using (var readStream = new StreamReader(inputStream, Encoding.UTF8))
            {
                return readStream.ReadToEnd();
            }
        }
    }
}
