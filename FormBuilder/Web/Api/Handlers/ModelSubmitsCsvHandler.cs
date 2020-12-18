using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace CompositeC1Contrib.FormBuilder.Web.Api.Handlers
{
    // formbuilder/{name}/submits.csv
    public class ModelSubmitsCsvHandler : FormBuilderHttpHandlerBase
    {
        private static readonly char[] SpecialChars = { ',', '\n', '\r', '"' };

        protected override void ProcessRequest()
        {
            if (!EnsureHttpMethod("GET"))
            {
                return;
            }

            if (!EnsureLoggedIn())
            {
                return;
            }

            var name = (string)Context.Request.RequestContext.RouteData.Values["name"];
            var submits = SaveSubmitFacade.LoadSubmits(name).ToList();

            var hasSubmits = submits.Any();
            if (!hasSubmits)
            {
                Context.Response.StatusCode = (int)HttpStatusCode.NotFound;

                return;
            }

            Context.Response.ContentType = "application/csv";
            Context.Response.Headers["Content-Disposition"] = $"attachment; filename=\"{name}.csv\"";

            using (var writer = new StreamWriter(Context.Response.OutputStream))
            {
                WriteByteOrderMark(writer);
                WriteHeader(submits, writer);

                foreach (var submit in submits)
                {
                    WriteItem(submit, writer);
                }
            }
        }

        private static void WriteByteOrderMark(TextWriter writer)
        {
            writer.Write("\xfeff");
        }

        private static void WriteHeader(IEnumerable<ModelSubmit> submits, TextWriter writer)
        {
            var distinctFieldNames = submits.SelectMany(s => s.Values).Select(v => v.Key).Distinct().ToList();

            distinctFieldNames.Add("Submitted time");

            var header = String.Join(",", distinctFieldNames);

            writer.WriteLine(header);
        }

        private static void WriteItem(ModelSubmit submit, TextWriter writer)
        {
            var values = submit.Values.Select(s => String.IsNullOrEmpty(s.Value) ? String.Empty : Escape(s.Value)).ToList();

            values.Add(submit.Time.ToString("o", CultureInfo.InvariantCulture));

            var line = String.Join(",", values);

            writer.WriteLine(line);
        }

        private static string Escape(string field)
        {
            if (field == null)
            {
                return String.Empty;
            }

            return field.IndexOfAny(SpecialChars) != -1 ? $"\"{field.Replace("\"", "\"\"")}\"" : field;
        }
    }
}
