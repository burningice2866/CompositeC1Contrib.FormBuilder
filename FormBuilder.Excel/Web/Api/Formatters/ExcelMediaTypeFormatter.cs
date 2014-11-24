using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using ClosedXML.Excel;

namespace CompositeC1Contrib.FormBuilder.Excel.Web.Api.Formatters
{
    public class ExcelMediaTypeFormatter : MediaTypeFormatter
    {
        public ExcelMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
            MediaTypeMappings.Add(new UriPathExtensionMapping("xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            return type == typeof(IEnumerable<FormSubmit>);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                var itm = value as IEnumerable<FormSubmit>;
                if (itm == null)
                {
                    throw new InvalidOperationException("Cannot serialize type");
                }

                var list = itm.ToList();
                var form = list[0].OwningForm;
                var table = GenerateDataTable(form, list);

                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add(list[0].OwningForm.Name);

                worksheet.Cell(1, 1).InsertTable(table);

                workbook.SaveAs(writeStream);
            });
        }

        private static DataTable GenerateDataTable(IFormModel form, IEnumerable<FormSubmit> submits)
        {
            var table = new DataTable();

            foreach (var field in form.Fields)
            {
                table.Columns.Add(field.Name, field.ValueType);
            }

            table.Columns.Add("Submitted time", typeof(DateTime));

            foreach (var submit in submits)
            {
                var row = table.NewRow();

                foreach (var value in submit.Values)
                {
                    var field = form.Fields.SingleOrDefault(f => f.Name == value.Key);
                    if (field == null)
                    {
                        continue;
                    }

                    try
                    {
                        var val = Convert.ChangeType(value.Value, field.ValueType);

                        row[value.Key] = val;
                    }
                    catch { }
                }

                row["Submitted time"] = submit.Time;

                table.Rows.Add(row);
            }

            return table;
        }
    }
}
