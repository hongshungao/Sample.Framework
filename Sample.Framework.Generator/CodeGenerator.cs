using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Scriban;
using Scriban.Runtime;
using Sample.Framework.Core.Models;

namespace Sample.Framework.Generator
{
    /// <summary>
    /// 代码生成器，使用Scriban模板引擎生成代码
    /// </summary>
    public class CodeGenerator
    {
        private readonly string _templateDirectory;
        private readonly string _outputDirectory;

        public CodeGenerator(string templateDirectory, string outputDirectory)
        {
            _templateDirectory = templateDirectory;
            _outputDirectory = outputDirectory;
        }

        /// <summary>
        /// 生成实体代码
        /// </summary>
        /// <param name="entity">实体定义</param>
        /// <returns>生成的文件路径</returns>
        public async Task<string> GenerateEntityCodeAsync(Entity entity)
        {
            // 加载实体模板
            var templatePath = Path.Combine(_templateDirectory, "Entity.scriban");
            var templateContent = await File.ReadAllTextAsync(templatePath);
            var template = Template.Parse(templateContent);

            // 创建模板上下文
            var scriptObject = new ScriptObject();
            scriptObject.Add("entity", entity);
            scriptObject.Add("namespace", entity.Namespace ?? "Sample.Generated");

            var context = new TemplateContext();
            context.PushGlobal(scriptObject);

            // 渲染模板
            var result = await template.RenderAsync(context);

            // 确保输出目录存在
            var entityDirectory = Path.Combine(_outputDirectory, "Entities");
            Directory.CreateDirectory(entityDirectory);

            // 写入文件
            var filePath = Path.Combine(entityDirectory, $"{entity.Name}.cs");
            await File.WriteAllTextAsync(filePath, result);

            return filePath;
        }

        /// <summary>
        /// 生成枚举代码
        /// </summary>
        /// <param name="enumDefinition">枚举定义</param>
        /// <returns>生成的文件路径</returns>
        public async Task<string> GenerateEnumCodeAsync(EnumDefinition enumDefinition)
        {
            // 加载枚举模板
            var templatePath = Path.Combine(_templateDirectory, "Enum.scriban");
            var templateContent = await File.ReadAllTextAsync(templatePath);
            var template = Template.Parse(templateContent);

            // 创建模板上下文
            var scriptObject = new ScriptObject();
            scriptObject.Add("enum", enumDefinition);
            scriptObject.Add("namespace", enumDefinition.Namespace ?? "Sample.Generated");

            var context = new TemplateContext();
            context.PushGlobal(scriptObject);

            // 渲染模板
            var result = await template.RenderAsync(context);

            // 确保输出目录存在
            var enumDirectory = Path.Combine(_outputDirectory, "Enums");
            Directory.CreateDirectory(enumDirectory);

            // 写入文件
            var filePath = Path.Combine(enumDirectory, $"{enumDefinition.Name}.cs");
            await File.WriteAllTextAsync(filePath, result);

            return filePath;
        }

        /// <summary>
        /// 批量生成代码
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <param name="enums">枚举列表</param>
        /// <returns>生成的文件路径列表</returns>
        public async Task<List<string>> GenerateCodeAsync(IEnumerable<Entity> entities, IEnumerable<EnumDefinition> enums)
        {
            var generatedFiles = new List<string>();

            // 生成枚举代码
            foreach (var enumDefinition in enums)
            {
                var filePath = await GenerateEnumCodeAsync(enumDefinition);
                generatedFiles.Add(filePath);
            }

            // 生成实体代码
            foreach (var entity in entities)
            {
                var filePath = await GenerateEntityCodeAsync(entity);
                generatedFiles.Add(filePath);
            }

            return generatedFiles;
        }
    }
}