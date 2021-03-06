﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace CompositeC1Contrib.FormBuilder
{
    public class SaveSubmitFacade
    {
        public static void SaveSubmitDebug(IModelInstance instance, HttpContextBase ctx)
        {
            var utcTime = DateTime.UtcNow;
            var timeStamp = utcTime.ToString("yyyy-MM-dd HH.mm.ss", CultureInfo.InvariantCulture);
            var dir = Path.Combine(ModelsFacade.RootPath, instance.Name, "Debug");
            var file = Path.Combine(dir, timeStamp + ".xml");
            var xml = GenerateBasicSubmitDocument(instance, utcTime);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var debugData = new XElement("debug",
                new XElement("url", ctx.Request?.Url?.ToString()),
                new XElement("clientIp", ctx.Request.UserHostAddress),
                new XElement("ua", ctx.Request.UserAgent));

            xml.AddFirst(debugData);

            xml.Save(EnsureUniqueFileName(file));
            SaveAttachments(instance, dir, timeStamp);
        }

        public static IEnumerable<ModelSubmit> LoadSubmits(string name)
        {
            var model = ModelsFacade.GetModel(name);
            var dir = Path.Combine(ModelsFacade.RootPath, name, "Submits");
            var files = Directory.GetFiles(dir, "*.xml");

            return files.Select(XElement.Load).Select(f => ModelSubmit.Parse(model, f));
        }

        public static void SaveSubmit(IModelInstance instance, bool includeAttachments)
        {
            SaveSubmit(instance, includeAttachments, DateTime.UtcNow);
        }

        public static void SaveSubmit(IModelInstance instance, bool includeAttachments, DateTime utcTime)
        {
            var timeStamp = utcTime.ToString("yyyy-MM-dd HH.mm.ss", CultureInfo.InvariantCulture);
            var dir = Path.Combine(ModelsFacade.RootPath, instance.Name, "Submits");
            var file = Path.Combine(dir, timeStamp + ".xml");
            var xml = GenerateBasicSubmitDocument(instance, utcTime);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            xml.Save(EnsureUniqueFileName(file));

            if (!includeAttachments || !instance.HasFileUpload)
            {
                return;
            }

            SaveAttachments(instance, dir, timeStamp);
        }

        private static XElement GenerateBasicSubmitDocument(IModelInstance instance, DateTime utcTime)
        {
            var xml = new XElement("formSubmit",
                new XAttribute("time", utcTime.ToString("o", CultureInfo.InvariantCulture)));

            var fields = new XElement("fields");

            foreach (var field in instance.Fields.Where(f => f.Label != null && f.Value != null))
            {
                string value;

                if (field.Value is DateTime dateTime)
                {
                    value = dateTime.ToString("o", CultureInfo.InvariantCulture);
                }
                else
                {
                    value = FormattingUtils.FormatFieldValue(field);
                }

                fields.Add(new XElement("field",
                    new XAttribute("name", field.Name),
                    new XAttribute("label", field.Label),
                    new XAttribute("value", value)));
            }

            xml.Add(fields);

            return xml;
        }

        private static void SaveAttachments(IModelInstance instance, string dir, string timeStamp)
        {
            var formFiles = new List<FormFile>();

            foreach (var field in instance.Fields)
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
