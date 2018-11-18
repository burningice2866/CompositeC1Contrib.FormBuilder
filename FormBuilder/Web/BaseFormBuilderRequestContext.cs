using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;

using CompositeC1Contrib.FormBuilder.Attributes;
using CompositeC1Contrib.FormBuilder.Configuration;
using CompositeC1Contrib.FormBuilder.Validation;
using CompositeC1Contrib.FormBuilder.Web.UI.Rendering;

namespace CompositeC1Contrib.FormBuilder.Web
{
    public abstract class BaseFormBuilderRequestContext
    {
        private static readonly Type RendererImplementation = FormBuilderConfiguration.GetSection().RendererImplementation;

        protected IModel Form { get; private set; }

        public FormRenderer FormRenderer { get; private set; }
        public ValidationResultList ValidationResult { get; protected set; }

        protected BaseFormBuilderRequestContext(IModel form)
        {
            ValidationResult = new ValidationResultList();
            Form = form;
            FormRenderer = (FormRenderer)Activator.CreateInstance(RendererImplementation);
        }

        protected virtual void OnMappedValues() { }
        public virtual void Submit() { }
    }

    public abstract class BaseFormBuilderRequestContext<T> : BaseFormBuilderRequestContext where T : IModelInstance
    {
        protected HttpContextBase HttpContext { get; private set; }
        public T ModelInstance { get; protected set; }

        public bool IsSuccess
        {
            get
            {
                if (!IsOwnSubmit)
                {
                    return false;
                }

                if (!ModelInstance.DisableAntiForgery)
                {
                    try
                    {
                        AntiForgery.Validate();
                    }
                    catch { return false; }
                }

                return !ValidationResult.Any();
            }
        }

        public bool IsOwnSubmit => HttpContext.Request.RequestType == "POST" && HttpContext.Request.Form["__type"] == ModelInstance.Name;

        protected BaseFormBuilderRequestContext(IModel form) : base(form) { }

        public virtual void Execute(HttpContextBase context)
        {
            HttpContext = context;

            var request = HttpContext.Request;

            if (!request.IsLocal && ModelInstance.ForceHttps && !request.IsSecureConnection && request.Url != null)
            {
                var redirectUrl = request.Url.ToString().Replace("http:", "https:");

                HttpContext.Response.Redirect(redirectUrl, true);

                return;
            }

            ModelInstance.SetDefaultValues();

            if (!IsOwnSubmit)
            {
                return;
            }

            var requestFiles = request.Files;
            var files = new List<FormFile>();

            for (var i = 0; i < requestFiles.Count; i++)
            {
                var f = requestFiles[i];
                if (f == null)
                {
                    continue;
                }

                if (f.ContentLength > 0)
                {
                    files.Add(new FormFile
                    {
                        Key = requestFiles.AllKeys[i],
                        ContentLength = f.ContentLength,
                        ContentType = f.ContentType,
                        FileName = Path.GetFileName(f.FileName),
                        InputStream = f.InputStream
                    });
                }
            }

            var postedValues = GetPostedValues(request);

            ModelInstance.MapValues(postedValues, files);
            OnMappedValues();

            ValidationResult = ModelInstance.Validate(new ValidationOptions());
        }

        private NameValueCollection GetPostedValues(HttpRequestBase request)
        {
            var nmc = new NameValueCollection();

            foreach (var field in ModelInstance.Fields)
            {
                var form = request.Form;

                if (field.Attributes.OfType<AllowHtmlAttribute>().Any())
                {
                    form = request.Unvalidated.Form;
                }

                var value = form[field.Name];

                nmc.Add(field.Name, value);
            }

            return nmc;
        }
    }
}
