using Scriban;
using Scriban.Runtime;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Framework.Generator
{
    public static class CodeHelper
    {
        public static void GenerateCode(string templatePath, string entityClassPath, string outputDirectory)
        {
            ValidatePaths(templatePath, outputDirectory);
            var entityType = GetEntityType(entityClassPath);
            var properties = GetEntityProperties(entityType);
            var template = LoadTemplate(templatePath);
            var result = RenderTemplate(template, entityType, properties);
            WriteOutput(outputDirectory, entityType, result);
        }

        public static void ValidatePaths(string templatePath, string outputDirectory)
        {
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"未找到模板文件: {templatePath}");
            }

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
        }

        public static Type GetEntityType(string entityClassPath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var entityType = assembly.GetTypes().FirstOrDefault(t => t.Name == Path.GetFileNameWithoutExtension(entityClassPath) && t.Namespace == "GenerateCode");

            if (entityType == null)
            {
                throw new Exception($"未找到实体类: {entityClassPath}");
            }

            return entityType;
        }

        public static List<object> GetEntityProperties(Type entityType)
        {
            return entityType.GetProperties()
                .Select(p => (object)new
                {
                    Name = p.Name,
                    Type = GetPropertyTypeName(p.PropertyType),
                    Description = (p.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "")
                })
                .ToList();
        }

        public static Template LoadTemplate(string templatePath)
        {
            var templateContent = File.ReadAllText(templatePath);
            return Template.Parse(templateContent);
        }

        public static string RenderTemplate(Template template, Type entityType, List<object> properties)
        {
            var scriptObject = new ScriptObject();
            scriptObject.Import(typeof(CodeHelper));
            scriptObject["EntityName"] = entityType.Name;
            scriptObject["Description"] = entityType.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "";
            scriptObject["Properties"] = properties;

            var context = new TemplateContext();
            context.PushGlobal(scriptObject);

            return template.Render(context);
        }

        public static void WriteOutput(string outputDirectory, Type entityType, string result)
        {
            var outputPath = Path.Combine(outputDirectory, $"{entityType.Name}Service.cs");
            File.WriteAllText(outputPath, result);
        }

        public static string GetPropertyTypeName(Type propertyType)
        {
            if (propertyType.IsGenericType)
            {
                var genericType = propertyType.GetGenericTypeDefinition();
                var typeArguments = propertyType.GetGenericArguments()
                    .Select(GetPropertyTypeName)
                    .ToArray();
                return $"{genericType.Name}<{string.Join(",", typeArguments)}>";
            }
            return propertyType.Name;
        }
    }
}
