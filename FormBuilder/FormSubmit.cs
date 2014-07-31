using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace CompositeC1Contrib.FormBuilder
{
    public class FormSubmit
    {
        public DateTime Time { get; set; }
        public IList<SubmitField> Values { get; set; }

        public FormSubmit()
        {
            Values = new List<SubmitField>();
        }

        public static FormSubmit Parse(XElement el)
        {
            var submit = new FormSubmit();

            foreach (var field in el.Descendants("field"))
            {
                var key = field.Attribute("name").Value;
                var value = field.Attribute("value").Value;

                submit.Values.Add(new SubmitField
                {
                    Key = key,
                    Value = value
                });
            }

            submit.Time = DateTime.Parse(el.Attribute("time").Value, CultureInfo.InvariantCulture);

            return submit;
        }
    }
}
