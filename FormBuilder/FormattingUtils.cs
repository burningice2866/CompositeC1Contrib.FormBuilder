using System;
using System.Collections.Generic;
using System.Linq;

namespace CompositeC1Contrib.FormBuilder
{
    public class FormattingUtils
    {
        public static string FormatFieldValue(FormField field)
        {
            if (field.Value == null)
            {
                return String.Empty;
            }

            if (field.Value is IEnumerable<string>)
            {
                return String.Join(", ", field.Value as IEnumerable<string>);
            }

            if (field.Value is FormFile)
            {
                var file = field.Value as FormFile;

                return GetFileFieldValue(file);
            }

            if (field.Value is IEnumerable<FormFile>)
            {
                var files = field.Value as IEnumerable<FormFile>;
                var values = files.Select(GetFileFieldValue).ToList();

                return String.Join(", ", values);
            }

            return field.Value.ToString();
        }

        private static string GetFileFieldValue(FormFile file)
        {
            return String.Format("{0} ({1} KB)", file.FileName, file.ContentLength / 1024);
        }
    }
}
