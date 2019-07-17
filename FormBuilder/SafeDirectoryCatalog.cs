using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CompositeC1Contrib.FormBuilder
{
    public class SafeDirectoryCatalog : ComposablePartCatalog
    {
        private readonly AggregateCatalog _catalog = new AggregateCatalog();

        public SafeDirectoryCatalog(string directory, ReflectionContext reflectionContext = null)
        {
            Initialize(directory, reflectionContext);
        }

        private void Initialize(string directory, ReflectionContext reflectionContext)
        {
            var files = Directory.EnumerateFiles(directory, "*.dll", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                try
                {
                    var catalog = reflectionContext == null ? new AssemblyCatalog(file) : new AssemblyCatalog(file, reflectionContext);

                    // Force MEF to load the plugin and figure out if there are any exports
                    // good assemblies will not throw the RTLE exception and can be added to the catalog
                    if (catalog.Parts.Any())
                    {
                        _catalog.Catalogs.Add(catalog);
                    }
                }
                catch (BadImageFormatException) { }
                catch (ReflectionTypeLoadException) { }
            }
        }

        public override IQueryable<ComposablePartDefinition> Parts => _catalog.Parts;
    }
}
