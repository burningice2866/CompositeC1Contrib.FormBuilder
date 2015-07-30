using Composite.Data;
using Composite.Data.Hierarchy;
using Composite.Data.Hierarchy.DataAncestorProviders;

namespace CompositeC1Contrib.FormBuilder.Data.Types
{
    [AutoUpdateble]
    [KeyPropertyName("Name")]
    [DataScope(DataScopeIdentifier.PublicName)]
    [ImmutableTypeId("b42a799b-300d-4c8d-9bc8-487642767fb8")]
    [Title("Formbuilder form")]
    [LabelPropertyName("Name")]
    [DataAncestorProvider(typeof(NoAncestorDataAncestorProvider))]
    public interface IModelReference : IData
    {
        [StoreFieldType(PhysicalStoreFieldType.String, 255)]
        [ImmutableFieldId("ea4ba3bd-1953-47cd-9f6b-3885c24bb972")]
        string Name { get; }
    }
}
