using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using CompositeC1Contrib.FormBuilder.Web.Api.Models;

namespace CompositeC1Contrib.FormBuilder.Web.Api.Controllers
{
    public class ValidationController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post()
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

        private IHttpActionResult CreateAndValidateForm(NameValueCollection form, IEnumerable<FormFile> files)
        {
            var model = CreateFormModel(form);

            model.SetDefaultValues();
            model.MapValues(form, files);

            var validationResult = model.Validate(false);

            if (!validationResult.Any())
            {
                return Ok(true);
            }

            var resultList = validationResult.Select(o => new ValidationError
            {
                AffectedFields = o.AffectedFormIds,
                Message = o.ValidationMessage
            });

            return Ok(resultList);
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
            var model = FormModelsFacade.GetModel(name);

            return model;
        }
    }
}
