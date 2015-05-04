using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using CompositeC1Contrib.FormBuilder.Attributes;

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

            var formatAttr = field.Attributes.OfType<DisplayFormatAttribute>().SingleOrDefault();

            var underlyingType = Nullable.GetUnderlyingType(field.ValueType);
            if (underlyingType != null && formatAttr != null)
            {
                var hasValue = (bool)field.ValueType.GetProperty("HasValue").GetValue(field.Value, null);
                if (hasValue)
                {
                    var value = field.ValueType.GetProperty("Value").GetValue(field.Value, null) as IFormattable;
                    if (value != null)
                    {
                        return value.ToString(formatAttr.FormatString, CultureInfo.CurrentUICulture);
                    }
                }
            }

            if (formatAttr == null || !(field.Value is IFormattable))
            {
                return field.Value.ToString();
            }

            return ((IFormattable)field.Value).ToString(formatAttr.FormatString, CultureInfo.CurrentUICulture);
        }

        private static string GetFileFieldValue(FormFile file)
        {
            return String.Format("{0} ({1} KB)", file.FileName, file.ContentLength / 1024);
        }
    }
}
