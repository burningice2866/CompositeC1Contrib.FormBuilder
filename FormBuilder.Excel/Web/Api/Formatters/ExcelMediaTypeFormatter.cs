using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
        private static readonly HashSet<Type> UsableFieldTypes = new HashSet<Type>
        {
            typeof(string),
            typeof(char),
            typeof(TimeSpan),
            typeof(DateTime),
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal)
        };

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
            return type == typeof(IEnumerable<ModelSubmit>);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                var itm = value as IEnumerable<ModelSubmit>;
                if (itm == null)
                {
                    throw new InvalidOperationException("Cannot serialize type");
                }

                var list = itm.ToList();
                var form = list[0].OwningForm;
                var table = GenerateDataTable(form, list);

                var formName = form.Name.Substring(form.Name.LastIndexOf(".", StringComparison.Ordinal) + 1);

                if (formName.Length > 31)
                {
                    formName = formName.Substring(0, 31);
                }

                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add(formName);

                worksheet.Cell(1, 1).InsertTable(table);
                worksheet.Range(2, 1, table.Rows.Count, table.Columns.Count).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                workbook.SaveAs(writeStream);
            });
        }

        private static DataTable GenerateDataTable(IModel model, IEnumerable<ModelSubmit> submits)
        {
            var table = new DataTable();

            foreach (var field in model.Fields)
            {
                var usableType = GetUsableFieldType(field);

                table.Columns.Add(field.Name, usableType);
            }

            table.Columns.Add("Submitted time", typeof(DateTime));

            foreach (var submit in submits)
            {
                var row = table.NewRow();

                foreach (var value in submit.Values)
                {
                    var field = model.Fields.SingleOrDefault(f => f.Name == value.Key);
                    if (field == null)
                    {
                        continue;
                    }

                    var usableType = GetUsableFieldType(field);

                    object val = null;

                    try
                    {
                        val = Convert.ChangeType(value.Value, usableType, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                        val = TryParseBackwardCompatibleFormat(usableType, value);
                    }

                    row[value.Key] = val;
                }

                row["Submitted time"] = submit.Time;

                table.Rows.Add(row);
            }

            return table;
        }

        private static object TryParseBackwardCompatibleFormat(Type usableType, SubmitField value)
        {
            if (usableType == typeof(DateTime))
            {
                var languagesToTry = new[] { new CultureInfo("en-US"), new CultureInfo("en-GB"), new CultureInfo("da-DK") };

                foreach (var ci in languagesToTry)
                {
                    try
                    {
                        return DateTime.Parse(value.Value, ci);
                    }
                    catch { }
                }
            }

            return null;
        }

        private static Type GetUsableFieldType(FormFieldModel field)
        {
            return !UsableFieldTypes.Contains(field.ValueType) ? typeof(string) : field.ValueType;
        }
    }
}
