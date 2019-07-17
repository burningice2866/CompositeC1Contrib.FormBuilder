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

            if (field.Value is IEnumerable<string> enumerable)
            {
                return String.Join(", ", enumerable);
            }

            if (field.Value is FormFile file)
            {
                return GetFileFieldValue(file);
            }

            if (field.Value is IEnumerable<FormFile> files)
            {
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
                    if (field.ValueType.GetProperty("Value").GetValue(field.Value, null) is IFormattable formattable)
                    {
                        return formattable.ToString(formatAttr.FormatString, CultureInfo.CurrentUICulture);
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
            return $"{file.FileName} ({file.ContentLength / 1024} KB)";
        }
    }
}
