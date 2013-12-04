﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;

using Composite.C1Console.Security;
using Composite.Core.Xml;
using Composite.Functions;

using CompositeC1Contrib.FormBuilder.Web.UI;

namespace CompositeC1Contrib.FormBuilder.FunctionProviders
{
    public class DumpSubmittedFormValues : IFunction
    {
        public string Namespace { get; private set; }
        public string Name { get; private set; }

        public EntityToken EntityToken
        {
            get { return new FormBuilderFunctionEntityToken(typeof(FormBuilderFunctionProvider).Name, Name); }
        }

        public string Description
        {
            get { return ""; }
        }

        public Type ReturnType
        {
            get { return typeof(XhtmlDocument); }
        }

        public IEnumerable<ParameterProfile> ParameterProfiles
        {
            get { return Enumerable.Empty<ParameterProfile>(); }
        }

        public DumpSubmittedFormValues()
        {
            Namespace = "FormBuilder";
            Name = "DumpSubmittedFormValues";
        }

        public object Execute(ParameterList parameters, FunctionContextContainer context)
        {
            var formModel = (FormModel)context.GetParameterValue("FormModel", typeof(FormModel));
            var html = new StringBuilder();

            html.Append("<table>");

            foreach (var field in formModel.Fields.Where(f => f.Label != null && f.Value != null))
            {
                var value = String.Empty;

                if (field.Value is IEnumerable<string>)
                {
                    value = String.Join(", ", field.Value as IEnumerable<string>);
                }
                else if (field.Value is FormFile)
                {
                    var file = field.Value as FormFile;
                    value = String.Format("{0} ({1} KB)", file.FileName, file.ContentLength / 1024);
                }
                else if (field.Value is IEnumerable<FormFile>)
                {
                    var files = field.Value as IEnumerable<FormFile>;
                    var values = new List<string>();
                    foreach (var file in files)
                    {
                        values.Add(String.Format("{0} ({1} KB)", file.FileName, file.ContentLength / 1024));
                    }
                    value = String.Join(", ", values);
                }
                else
                {
                    value = field.Value.ToString();
                }
                    
                value = HttpUtility.HtmlEncode(value);

                if (field.InputElementType is TextAreaInputElementAttribute)
                {
                    value = value.Replace("\r", String.Empty).Replace("\n", "<br />");
                }

                html.AppendFormat("<tr><td>{0}:</td><td>{1}</td></tr>", HttpUtility.HtmlEncode(field.Label.Label), value);
            }

            html.Append("</table>");

            var doc = new XhtmlDocument();
            doc.Body.Add(XElement.Parse(html.ToString()));

            return doc;
        }
    }
}
