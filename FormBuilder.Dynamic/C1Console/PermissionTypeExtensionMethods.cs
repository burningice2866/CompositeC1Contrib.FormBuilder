using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console
{
    public static class PermissionTypeExtensionMethods
    {
        public static IEnumerable<PermissionType> FromListOfStrings(this IEnumerable<string> permissionTypeNames)
        {
            if (permissionTypeNames == null) throw new ArgumentNullException("permissionTypeNames");

            return permissionTypeNames.Select(permissionTypeName => (PermissionType)Enum.Parse(typeof(PermissionType), permissionTypeName));
        }

        public static string SerializePermissionTypes(this IEnumerable<PermissionType> permissionTypes)
        {
            if (permissionTypes == null) throw new ArgumentNullException("permissionTypes");

            return String.Join("�", permissionTypes);
        }

        public static IEnumerable<PermissionType> DeserializePermissionTypes(this string serializedPermissionTypes)
        {
            if (serializedPermissionTypes == null) throw new ArgumentNullException("serializedPermissionTypes");

            var split = serializedPermissionTypes.Split(new[] { '�' }, StringSplitOptions.RemoveEmptyEntries);

            return split.Select(s => (PermissionType)Enum.Parse(typeof(PermissionType), s));
        }
    }
}
