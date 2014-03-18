using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using CompositeC1Contrib.FormBuilder.Web.Api.Models;

namespace CompositeC1Contrib.FormBuilder.Web.Api.Controllers
{
    public class ValidationController : ApiController
    {
        [HttpPost]
        public Task<HttpResponseMessage> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return Request.Content.ReadAsFormDataAsync().ContinueWith(t =>
                {
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                    }

                    return CreateAndValidateForm(t.Result, Enumerable.Empty<FormFile>());
                });
            }

            var path = Path.GetTempPath();
            var provider = new MultipartFormDataStreamProvider(path);

            return Request.Content.ReadAsMultipartAsync(provider).ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                {
                    Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                }

                var files = new List<FormFile>();

                foreach (var file in t.Result.FileData)
                {
                    var fi = new FileInfo(file.LocalFileName);

                    files.Add(new FormFile()
                    {
                        Key = file.Headers.ContentDisposition.Name.Replace("\"", String.Empty),
                        ContentLength = (int)fi.Length,
                        ContentType = file.Headers.ContentType.ToString(),
                        FileName = file.Headers.ContentDisposition.FileName.Replace("\"", String.Empty),
                        InputStream = File.OpenRead(fi.FullName)
                    });
                }

                return CreateAndValidateForm(t.Result.FormData, files);
            });
        }

        private HttpResponseMessage CreateAndValidateForm(NameValueCollection form, IEnumerable<FormFile> files)
        {
            var model = CreateFormModel(form);

            model.MapValues(form, files);
            model.Validate();

            if (!model.ValidationResult.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, true);
            }

            var resultList = model.ValidationResult.Select(o => new ValidationError()
            {
                AffectedFields = o.AffectedFormIds,
                Message = o.ValidationMessage
            });

            return Request.CreateResponse(HttpStatusCode.OK, resultList);
        }

        private FormModel CreateFormModel(NameValueCollection form)
        {
            string name = form["__type"];
            var model = FormModelsFacade.GetModels().Single(f => f.Name == name);

            return model;
        }
    }
}
