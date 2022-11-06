using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CaaS.Util
{
    public class ReflectionUtil
    {
        private const string CORELIB = "System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e";
        private const string SYSTEMCORE = "System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        private const string MSCORELIB = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

        /// <summary>
        /// Checks if a property has an anonymous type 
        /// <see href="https://stackoverflow.com/questions/2483023/how-to-test-if-a-type-is-anonymous"></see>
        /// </summary>
        /// <param name="property">The property to check</param>
        /// <returns></returns>
        public static bool IsAnonymousType(PropertyInfo? property)
        {
            if (property is null)
            {
                return false;
            }
            var type = property.PropertyType;
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && type.Attributes.HasFlag(TypeAttributes.NotPublic);
        }

        /// <summary>
        /// Checks if a property is a system type
        /// <see href="https://stackoverflow.com/questions/3174921/how-do-i-determine-if-system-type-is-a-custom-type-or-a-framework-type"></see>
        /// </summary>
        /// <param name="property">The property to check</param>
        /// <returns></returns>
        public static bool IsSystemType(PropertyInfo? property)
        {
            if (property is null)
            {
                return false;
            }

            var systemNames = new List<AssemblyName>()
            {
                new AssemblyName (MSCORELIB),
                new AssemblyName (SYSTEMCORE),
                new AssemblyName (CORELIB)
            };

            var assemblyFullName = property.GetType().Assembly.FullName;

            if (assemblyFullName is null)
            {
                return false;
            }

            var assemblyName = new AssemblyName(assemblyFullName);

            return !IsAnonymousType(property) && systemNames.Any(
                    systemName => systemName.GetPublicKeyToken() != null && assemblyName != null && assemblyName.GetPublicKeyToken() != null &&
                    systemName.Name == assemblyName.Name && systemName.GetPublicKeyToken()!.SequenceEqual(assemblyName.GetPublicKeyToken()!));
        }

        /// <summary>
        /// Retrieves the Property Name either by its name or by an DisplayName set to the property
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static string? GetPropertyName(PropertyInfo propertyInfo)
        {
            var attribute = (DisplayAttribute?)propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault();

            if (attribute is null)
            {
                return propertyInfo.Name;
            }

            return attribute.Name;
        }
    }
}