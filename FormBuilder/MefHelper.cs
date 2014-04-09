using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;

namespace CompositeC1Contrib.FormBuilder
{
    public static class MefHelper
    {
        public static IEnumerable<Type> GetExportedTypes<T>(DirectoryCatalog catalog)
        {
            return catalog.Parts
                .Select(part => ComposablePartExportType<T>(part))
                .Where(t => t != null);
        }

        private static Type ComposablePartExportType<T>(ComposablePartDefinition part)
        {
            return part.ExportDefinitions.Any(DefintionMatch<T>) ? ReflectionModelServices.GetPartType(part).Value : null;
        }

        private static bool DefintionMatch<T>(ExportDefinition def)
        {
            return def.Metadata.ContainsKey("ExportTypeIdentity") && def.Metadata["ExportTypeIdentity"].Equals(typeof(T).FullName);
        }
    }
}
