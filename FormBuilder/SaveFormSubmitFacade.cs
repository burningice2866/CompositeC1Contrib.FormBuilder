using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using CompositeC1Contrib.FormBuilder.FunctionProviders;

namespace CompositeC1Contrib.FormBuilder
{
    public class SaveFormSubmitFacade
    {
        public static void SaveSubmitDebug(FormModel model)
        {
            var ctx = HttpContext.Current;
            var time = DateTime.UtcNow;
            var timeStamp = time.ToString("yyyy-MM-dd HH.mm.ss", CultureInfo.InvariantCulture);
            var dir = Path.Combine(FormModelsFacade.FormsPath, model.Name, "Debug");
            var file = Path.Combine(dir, timeStamp + ".xml");
            var xml = GenerateBasicSubmitDocument(model, time);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var debugData = new XElement("debug",
                new XElement("url", ctx.Request.Url.ToString()),
                new XElement("clientIp", ctx.Request.UserHostAddress),
                new XElement("ua", ctx.Request.UserAgent));

            xml.AddFirst(debugData);

            xml.Save(EnsureUniqueFileName(file));
            SaveAttachments(model, dir, timeStamp);
        }

        public static void SaveSubmit(FormModel model, bool includeAttachments)
        {
            SaveSubmit(model, includeAttachments, DateTime.UtcNow);
        }

        public static void SaveSubmit(FormModel model, bool includeAttachments, DateTime time)
        {
            var timeStamp = time.ToString("yyyy-MM-dd HH.mm.ss", CultureInfo.InvariantCulture);
            var dir = Path.Combine(FormModelsFacade.FormsPath, model.Name, "Submits");
            var file = Path.Combine(dir, timeStamp + ".xml");
            var xml = GenerateBasicSubmitDocument(model, time);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            xml.Save(EnsureUniqueFileName(file));

            if (!includeAttachments || !model.HasFileUpload)
            {
                return;
            }

            SaveAttachments(model, dir, timeStamp);
        }

        private static XElement GenerateBasicSubmitDocument(FormModel model, DateTime time)
        {
            var xml = new XElement("formSubmit",
                new XAttribute("time", time.ToString(CultureInfo.InvariantCulture)));

            var fields = new XElement("fields");

            foreach (var field in model.Fields.Where(f => f.Label != null && f.Value != null))
            {
                var value = DumpSubmittedFormValues.FormatFieldValue(field);

                fields.Add(new XElement("field",
                    new XAttribute("name", field.Name),
                    new XAttribute("label", field.Label.Label),
                    new XAttribute("value", value)));
            }

            xml.Add(fields);

            return xml;
        }

        private static void SaveAttachments(FormModel model, string dir, string timeStamp)
        {
            var formFiles = new List<FormFile>();

            foreach (var field in model.Fields)
            {
                if (field.ValueType == typeof(FormFile) && field.Value != null)
                {
                    formFiles.Add((FormFile)field.Value);
                }
                else if (field.ValueType == typeof(IEnumerable<FormFile>) && field.Value != null)
                {
                    formFiles.AddRange((IEnumerable<FormFile>)field.Value);
                }
            }

            foreach (var formFile in formFiles)
            {
                var file = Path.Combine(dir, timeStamp + "_" + formFile.FileName);
                file = EnsureUniqueFileName(file);

                formFile.InputStream.Seek(0, SeekOrigin.Begin);

                using (var fileStream = File.Create(file))
                {
                    formFile.InputStream.CopyTo(fileStream);
                }
            }
        }

        public static string EnsureUniqueFileName(string file)
        {
            var dir = Path.GetDirectoryName(file);
            var fileName = Path.GetFileName(file);

            while (Directory.GetFiles(dir, fileName, SearchOption.TopDirectoryOnly).Any())
            {
                fileName = Path.GetFileNameWithoutExtension(fileName) + "_1" + Path.GetExtension(fileName);
            }

            return Path.Combine(dir, fileName);
        }
    }
}
