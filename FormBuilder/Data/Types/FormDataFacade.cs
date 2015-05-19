using System.Linq;

using Composite.Data;

namespace CompositeC1Contrib.FormBuilder.Data.Types
{
    public class FormDataFacade
    {
        public static IForm GetFormData(string name)
        {
            using (var data = new DataConnection())
            {
                return data.Get<IForm>().SingleOrDefault(f => f.Name == name);
            }
        }
    }
}
