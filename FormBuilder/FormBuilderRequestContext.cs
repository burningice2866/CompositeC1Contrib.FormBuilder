using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CompositeC1Contrib.FormBuilder
{
    public class FormBuilderRequestContext
    {
        private HttpContextBase ctx;

        private Action<FormModel> OnMappedValues;
        private Action<FormModel> SetDefaultValues;
        private Action OnSubmit;        

        public bool IsOwnSubmit
        {
            get { return ctx.Request.RequestType == "POST" && ctx.Request.Form["__type"] == RenderingModel.Name; }
        }

        public bool IsSuccess
        {
            get { return IsOwnSubmit && !RenderingModel.ValidationResult.Any(); }
        }

        public FormModel RenderingModel { get; private set; }

        private FormBuilderRequestContext(FormModel model, Action<FormModel> OnMappedValues, Action OnSubmit, Action<FormModel> SetDefaultValues)
        {
            ctx = new HttpContextWrapper(HttpContext.Current);

            RenderingModel = model;
            FormModel.SetCurrent(RenderingModel.Name, RenderingModel);

            this.OnMappedValues = OnMappedValues;
            this.SetDefaultValues = SetDefaultValues;
            this.OnSubmit = OnSubmit;
        }

        public static FormBuilderRequestContext Setup(FormModel model, Action<FormModel> OnMappedValues, Action OnSubmit, Action<FormModel> SetDefaultValues)
        {
            var requestContext = new FormBuilderRequestContext(model, OnMappedValues, OnSubmit, SetDefaultValues);

            var Request = requestContext.ctx.Request;
            var Response = requestContext.ctx.Response;

            if (requestContext.RenderingModel.ForceHttps && !Request.IsSecureConnection)
            {
                string redirectUrl = Request.Url.ToString().Replace("http:", "https:");

                Response.Redirect(redirectUrl, true);
            }

            if (requestContext.IsOwnSubmit)
            {
                var requestFiles = Request.Files;
                var files = new List<FormFile>();

                for (int i = 0; i < requestFiles.Count; i++)
                {
                    var f = requestFiles[i];
                    if (f.ContentLength > 0)
                    {
                        files.Add(new FormFile()
                        {
                            Key = requestFiles.AllKeys[i],
                            ContentLength = f.ContentLength,
                            ContentType = f.ContentType,
                            FileName = f.FileName,
                            InputStream = f.InputStream
                        });
                    }
                }

                requestContext.RenderingModel.MapValues(Request.Form, files);

                if (OnMappedValues != null)
                {
                    OnMappedValues(requestContext.RenderingModel);
                }

                requestContext.RenderingModel.Validate();
            }
            else
            {
                if (SetDefaultValues != null)
                {
                    SetDefaultValues(model);
                }
            }

            return requestContext;
        }

        public void Submit()
        {
            OnSubmit();
        }
    }
}
