using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Composite;

namespace CompositeC1Contrib.FormBuilder.Dynamic
{
    public class XElementHelper
    {
        public static object DeserializeInstanceWithArgument(Type type, XElement element)
        {
            Verify.ArgumentNotNull(type, "type");

            var ctors = type.GetConstructors().ToDictionary(ctor => ctor, ctor => ctor.GetParameters()).Where(o => o.Value.Any()).OrderByDescending(o => o.Value.Count());

            foreach (var kvp in ctors)
            {
                var parameters = kvp.Value;
                var ctorParams = new Dictionary<string, object>();

                foreach (var param in parameters)
                {
                    var attr = element.Attributes().SingleOrDefault(o => o.Name.LocalName.Equals(param.Name, StringComparison.OrdinalIgnoreCase));
                    if (attr == null)
                    {
                        break;
                    }

                    var name = param.Name;
                    object value = Convert.ChangeType(attr.Value, param.ParameterType);

                    ctorParams.Add(name, value);
                }

                if (ctorParams.Count == parameters.Length)
                {
                    return kvp.Key.Invoke(ctorParams.Values.ToArray());
                }
            }

            var instance = Activator.CreateInstance(type);

            foreach (var prop in type.GetProperties().Where(p => p.CanWrite))
            {
                var attr = element.Attributes().SingleOrDefault(o => o.Name.LocalName.Equals(prop.Name, StringComparison.OrdinalIgnoreCase));
                if (attr != null)
                {
                    prop.SetValue(instance, Convert.ChangeType(attr.Value, prop.PropertyType), null);

                    continue;
                }

                var node = element.Elements().SingleOrDefault(o => o.Name.LocalName.Equals(prop.Name, StringComparison.OrdinalIgnoreCase));
                if (node == null)
                {
                    continue;
                }

                prop.SetValue(instance, Convert.ChangeType(node.Value, prop.PropertyType), null);
            }

            return instance;
        }
    }
}
