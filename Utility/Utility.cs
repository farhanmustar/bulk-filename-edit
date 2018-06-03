using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BulkFIlenameEdit.Utility
{
    public static class Utility
    {

    }

    public static class Extension
    {
        //TODO: instead of duplication explore on targetting generic type that implement ICustomAttributeProvider.
        public static string GetDisplayName(this Type type)
        {
            var name = type.Name;

            var attr = type.GetCustomAttribute<DisplayNameAttribute>();
            if (attr != null)
            {
                var nameAttr = attr as DisplayNameAttribute;
                if (nameAttr != null)
                {
                    name = nameAttr.DisplayName;
                }
            }
            return name;
        }

        public static string GetDescription(this Type type)
        {
            var desc = "";

            var attr = type.GetCustomAttribute< DescriptionAttribute>();
            if (attr != null)
            {
                var descAttr = attr as DescriptionAttribute;
                if (descAttr != null)
                {
                    desc = descAttr.Description;
                }
            }
            return desc;
        }
        public static string GetDisplayName(this PropertyInfo propInfo)
        {
            var name = propInfo.Name;

            var attr = propInfo.GetCustomAttribute(typeof(DisplayNameAttribute));
            if (attr != null)
            {
                var nameAttr = attr as DisplayNameAttribute;
                if (nameAttr != null)
                {
                    name = nameAttr.DisplayName;
                }
            }
            return name;
        }

        public static string GetDescription(this PropertyInfo propInfo)
        {
            var desc = "";

            var attr = propInfo.GetCustomAttribute(typeof(DescriptionAttribute));
            if (attr != null)
            {
                var descAttr = attr as DescriptionAttribute;
                if (descAttr != null)
                {
                    desc = descAttr.Description;
                }
            }
            return desc;
        }

        /// <summary>
        /// Extension for 'Object' that copies the properties to a destination object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public static void CopyProperties(this object source, object destination)
        {
            // If any this null throw an exception
            if (source == null || destination == null)
                throw new Exception("Source or/and Destination Objects are null");
            // Getting the Types of the objects
            Type typeDest = destination.GetType();
            Type typeSrc = source.GetType();

            // Iterate the Properties of the source instance and  
            // populate them from their desination counterparts  
            PropertyInfo[] srcProps = typeSrc.GetProperties();
            foreach (PropertyInfo srcProp in srcProps)
            {
                if (!srcProp.CanRead)
                {
                    continue;
                }
                PropertyInfo targetProperty = typeDest.GetProperty(srcProp.Name);
                if (targetProperty == null)
                {
                    continue;
                }
                if (!targetProperty.CanWrite)
                {
                    continue;
                }
                if (targetProperty.GetSetMethod(true) != null && targetProperty.GetSetMethod(true).IsPrivate)
                {
                    continue;
                }
                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                {
                    continue;
                }
                if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType))
                {
                    continue;
                }
                // Passed all tests, lets set the value
                targetProperty.SetValue(destination, srcProp.GetValue(source, null), null);
            }
        }
    }
}
