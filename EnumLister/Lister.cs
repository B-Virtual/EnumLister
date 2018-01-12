using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using EnumLister.Model;

namespace EnumLister
{
    public class Lister
    {
        public static List<EnumModel> GenerateList(string[] assemblyNameFilters = null, string[] namespaceFilters = null)
        {
            var result = new List<EnumModel>();

            //string enumAssembly = "Pms.ServiceManagement.Data";
            //string enumNamespace = "Pms.ServiceManagement.Data.Interfaces.Entities";

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Select(x => x);

            if(assemblyNameFilters != null)
                assemblies = assemblies.Where(a => assemblyNameFilters.Any(f => a.FullName.Contains(f)));

            var q = assemblies.SelectMany(t => t.GetTypes());

            if(namespaceFilters != null)
                q = q.Where(t => t.IsEnum && t.Namespace != null && namespaceFilters.Any(f => t.Namespace.Contains(f))).ToList();

            foreach (var type in q)
            {
                var m = new EnumModel
                {
                    Name = type.Name,
                    Items = new List<EnumItemModel>()
                };

                var descAttrib = type.GetCustomAttribute(typeof(DescriptionAttribute));
                if (descAttrib != null)
                    m.Description = ((DescriptionAttribute) descAttrib).Description;

                // Get the different values for the Enum
                var fields = type.GetFields();
                foreach (var fieldInfo in fields)
                {
                    if (fieldInfo.IsLiteral)
                    {
                        var e = Enum.Parse(type, fieldInfo.GetValue(null).ToString()) as Enum;

                        m.Items.Add(new EnumItemModel
                        {
                            Value = Convert.ToInt32(e),
                            Name = e.ToString(),
                            Description = GetDescriptionAttributeValue(fieldInfo)
                        });
                    }
                }

                result.Add(m);
            }

            return result;
        }

        private static string GetDescriptionAttributeValue(FieldInfo fieldInfo)
        {
            var attribs = fieldInfo.GetCustomAttributes(true);

            var desc = attribs.FirstOrDefault(x => x is DescriptionAttribute);

            if (desc != null)
                return ((DescriptionAttribute)desc).Description;

            return string.Empty;
        }

    }
}
