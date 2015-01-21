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
        public HttpResponseMessage Post()
        {
            if (Request.Content.IsMimeMultipartContent())
            {
                var path = Path.GetTempPath();
                var provider = new MultipartFormDataStreamProvider(path);

                var multipartData = GetTaskResultSynchronously(Request.Content.ReadAsMultipartAsync(provider));
                var files = ParsePostedFiles(multipartData).ToList();

                return CreateAndValidateForm(multipartData.FormData, files);
            }

            var formData = GetTaskResultSynchronously(Request.Content.ReadAsFormDataAsync());

            return CreateAndValidateForm(formData, Enumerable.Empty<FormFile>());
        }

        private HttpResponseMessage CreateAndValidateForm(NameValueCollection form, IEnumerable<FormFile> files)
        {
            var model = CreateFormModel(form);

            model.SetDefaultValues();
            model.MapValues(form, files);
            model.Validate(false);

            if (!model.ValidationResult.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, true);
            }

            var resultList = model.ValidationResult.Select(o => new ValidationError
            {
                AffectedFields = o.AffectedFormIds,
                Message = o.ValidationMessage
            });

            return Request.CreateResponse(HttpStatusCode.OK, resultList);
        }

        private static IEnumerable<FormFile> ParsePostedFiles(MultipartFileStreamProvider multipartData)
        {
            var files = new List<FormFile>();

            foreach (var file in multipartData.FileData)
            {
                var fi = new FileInfo(file.LocalFileName);

                files.Add(new FormFile
                {
                    Key = file.Headers.ContentDisposition.Name.Replace("\"", String.Empty),
                    ContentLength = (int)fi.Length,
                    ContentType = file.Headers.ContentType.ToString(),
                    FileName = file.Headers.ContentDisposition.FileName.Replace("\"", String.Empty),
                    InputStream = File.OpenRead(fi.FullName)
                });
            }

            return files;
        }

        private static T GetTaskResultSynchronously<T>(Task<T> action)
        {
            return Task.Run(async () => await action).Result;
        }

        private static IFormModel CreateFormModel(NameValueCollection form)
        {
            var name = form["__type"];
            var model = FormModelsFacade.GetModels().Single(f => f.Name == name);

            return model;
        }
    }
}
