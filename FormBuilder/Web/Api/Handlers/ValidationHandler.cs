using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;

using CompositeC1Contrib.FormBuilder.Validation;
using CompositeC1Contrib.FormBuilder.Web.Api.Models;

using Newtonsoft.Json;

namespace CompositeC1Contrib.FormBuilder.Web.Api.Handlers
{
    // formbuilder/validation
    public class ValidationHandler : FormBuilderHttpHandlerBase
    {
        protected override void ProcessRequest()
        {
            if (!EnsureHttpMethod("POST"))
            {
                return;
            }

            var files = ParsePostedFiles(Context.Request.Files).ToList();
            var result = CreateAndValidateForm(Context.Request.Form, files);

            var body = JsonConvert.SerializeObject(result);

            Context.Response.StatusCode = (int)HttpStatusCode.OK;
            Context.Response.Write(body);
        }

        private static object CreateAndValidateForm(NameValueCollection form, IEnumerable<FormFile> files)
        {
            var instance = CreateFormInstance(form);

            instance.SetDefaultValues();
            instance.MapValues(form, files);

            var validationResult = instance.Validate(new ValidationOptions
            {
                ValidateFiles = false,
                ValidateCaptcha = false
            });

            if (!validationResult.Any())
            {
                return true;
            }

            var resultList = validationResult.Select(o => new ValidationError
            {
                AffectedFields = o.AffectedFormIds,
                Message = o.ValidationMessage
            });

            return resultList;
        }

        private static IEnumerable<FormFile> ParsePostedFiles(HttpFileCollectionBase files)
        {
            var list = new List<FormFile>();

            foreach (var key in files.AllKeys)
            {
                var file = files[key];

                list.Add(new FormFile
                {
                    Key = file.FileName.Replace("\"", String.Empty),
                    ContentLength = file.ContentLength,
                    ContentType = file.ContentType,
                    FileName = file.FileName.Replace("\"", String.Empty),
                    InputStream = file.InputStream
                });
            }

            return list;
        }

        private static IModelInstance CreateFormInstance(NameValueCollection form)
        {
            var name = form["__type"];
            var model = ModelsFacade.GetModel<FormModel>(name);

            return new Form(model);
        }
    }
}
