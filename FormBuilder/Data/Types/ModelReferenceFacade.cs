using System.Linq;

using Composite.Data;

namespace CompositeC1Contrib.FormBuilder.Data.Types
{
    public class ModelReferenceFacade
    {
        public static IModelReference GetModelReference(string name)
        {
            using (var data = new DataConnection())
            {
                return data.Get<IModelReference>().SingleOrDefault(f => f.Name == name);
            }
        }
    }
}
