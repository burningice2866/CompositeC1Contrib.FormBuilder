using System;
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
    public abstract class BaseDumpSubmittedFormValuesFunction : IFunction
    {
        public string Namespace
        {
            get { return "FormBuilder"; }
        }

        public string Name { get; protected set; }

        public EntityToken EntityToken
        {
            get { return new FormBuilderFunctionEntityToken(typeof(FormBuilderFunctionProvider).Name, Namespace + "." + Name); }
        }

        public string Description
        {
            get { return String.Empty; }
        }

        public Type ReturnType
        {
            get { return typeof(XhtmlDocument); }
        }

        public IEnumerable<ParameterProfile> ParameterProfiles
        {
            get { return Enumerable.Empty<ParameterProfile>(); }
        }

        public abstract object Execute(ParameterList parameters, FunctionContextContainer context);

        public static void DumpModelValues(IFormModel model, XhtmlDocument doc)
        {
            var html = new StringBuilder();

            html.Append("<table>");

            foreach (var field in model.Fields.Where(f => f.Label != null && f.Value != null))
            {
                var value = FormatFieldValue(field);

                value = HttpUtility.HtmlEncode(value);

                if (field.InputElementType is TextAreaInputElementAttribute)
                {
                    value = value.Replace("\r", String.Empty).Replace("\n", "<br />");
                }

                html.AppendFormat("<tr><td>{0}:</td><td>{1}</td></tr>", HttpUtility.HtmlEncode(field.Label.Label), value);
            }

            html.Append("</table>");

            doc.Body.Add(XElement.Parse(html.ToString()));
        }

        public static string FormatFieldValue(FormField field)
        {
            if (field.Value is IEnumerable<string>)
            {
                return String.Join(", ", field.Value as IEnumerable<string>);
            }

            if (field.Value is FormFile)
            {
                var file = field.Value as FormFile;

                return String.Format("{0} ({1} KB)", file.FileName, file.ContentLength / 1024);
            }

            if (field.Value is IEnumerable<FormFile>)
            {
                var files = field.Value as IEnumerable<FormFile>;
                var values = new List<string>();

                foreach (var file in files)
                {
                    values.Add(String.Format("{0} ({1} KB)", file.FileName, file.ContentLength / 1024));
                }

                return String.Join(", ", values);
            }

            return field.Value.ToString();
        }
    }
}
