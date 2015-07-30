using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CompositeC1Contrib.FormBuilder.Web.Api.Formatters
{
    public class CsvMediaTypeFormatter : MediaTypeFormatter
    {
        static readonly char[] SpecialChars = { ',', '\n', '\r', '"' };

        public CsvMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));
            MediaTypeMappings.Add(new UriPathExtensionMapping("csv", "text/csv"));
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            return type == typeof(IEnumerable<ModelSubmit>);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var writer = new StreamWriter(writeStream))
                {
                    var itm = value as IEnumerable<ModelSubmit>;
                    if (itm == null)
                    {
                        throw new InvalidOperationException("Cannot serialize type");
                    }

                    WriteByteOrderMark(writer);
                    WriteHeader(itm, writer);

                    foreach (var submit in itm)
                    {
                        WriteItem(submit, writer);
                    }

                }
            });
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

            return field.IndexOfAny(SpecialChars) != -1 ? String.Format("\"{0}\"", field.Replace("\"", "\"\"")) : field;
        }
    }
}
