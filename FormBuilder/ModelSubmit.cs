using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace CompositeC1Contrib.FormBuilder
{
    public class ModelSubmit
    {
        public IModel OwningForm { get; set; }
        public DateTime Time { get; set; }
        public IList<SubmitField> Values { get; set; }

        public ModelSubmit()
        {
            Values = new List<SubmitField>();
        }

        public static ModelSubmit Parse(IModel owner, XElement el)
        {
            var submit = new ModelSubmit
            {
                OwningForm = owner
            };

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

            var submittedTime = el.Attribute("time").Value;

            try
            {
                submit.Time = DateTime.Parse(submittedTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            }
            catch (FormatException)
            {
                submit.Time = DateTime.Parse(submittedTime, CultureInfo.InvariantCulture);
            }

            return submit;
        }
    }
}
