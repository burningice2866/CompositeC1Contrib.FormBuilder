using System;
using System.Collections.Generic;
using System.Text;

using Composite.C1Console.Security;

namespace CompositeC1Contrib.FormBuilder.Dynamic.C1Console.Tokens
{
    internal static class PermissionTypeExtensionMethods
    {
        public static IEnumerable<PermissionType> FromListOfStrings(this IEnumerable<string> permissionTypeNames)
        {
            if (permissionTypeNames == null) throw new ArgumentNullException("permissionTypeNames");

            foreach (string permissionTypeName in permissionTypeNames)
            {
                PermissionType permissionType = (PermissionType)Enum.Parse(typeof(PermissionType), permissionTypeName);

                yield return permissionType;
            }
        }

        public static string SerializePermissionTypes(this IEnumerable<PermissionType> permissionTypes)
        {
            if (permissionTypes == null) throw new ArgumentNullException("permissionType");

            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (PermissionType permissionType in permissionTypes)
            {
                if (first == false) sb.Append("�");
                else first = false;

                sb.Append(permissionType.ToString());
            }

            return sb.ToString();
        }

        public static IEnumerable<PermissionType> DeserializePermissionTypes(this string serializedPermissionTypes)
        {
            if (serializedPermissionTypes == null) throw new ArgumentNullException("serializedPermissionTypes");

            string[] split = serializedPermissionTypes.Split(new[] { '�' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string s in split)
            {
                yield return (PermissionType)Enum.Parse(typeof(PermissionType), s);
            }
        }
    }
}
