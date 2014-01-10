﻿using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CompositeC1Contrib.FormBuilder
{
    public abstract class FormBuilderRequestContext
    {
        private readonly HttpContextBase _ctx = new HttpContextWrapper(HttpContext.Current);

        protected string FormName { get; private set; }

        public bool IsOwnSubmit
        {
            get { return ctx.Request.RequestType == "POST" && ctx.Request.Form["__type"] == RenderingModel.Name; }
        }

        public bool IsSuccess
        {
            get { return IsOwnSubmit && !RenderingModel.ValidationResult.Any(); }
        }

        public abstract FormModel RenderingModel { get; }

        public FormBuilderRequestContext(string name)
        {
            FormName = name;
        }

        public void Execute()
        {
            FormModel.SetCurrent(RenderingModel.Name, RenderingModel);

            var Request = ctx.Request;
            var Response = ctx.Response;

            if (RenderingModel.ForceHttps && !Request.IsSecureConnection)
            {
                string redirectUrl = Request.Url.ToString().Replace("http:", "https:");

                Response.Redirect(redirectUrl, true);
            }

            SetDefaultValues();

            if (IsOwnSubmit)
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

                
                RenderingModel.MapValues(Request.Form, files);
                OnMappedValues();
                RenderingModel.Validate();
            }
        }

        public virtual void OnMappedValues() { }
        public virtual void SetDefaultValues() { }
        public virtual void OnSubmit() { }

        public void Submit()
        {
            OnSubmit();
        }
    }
}
