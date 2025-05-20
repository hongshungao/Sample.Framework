using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Scriban;
using Scriban.Runtime;

namespace Sample.Framework.Generator
{
    public class RoslynCodeHelper
    {
        // 主方法：根据模板和实体类生成代码
        public static void GenerateCode(string templatePath, string entityClassPath, string outputDirectory)
        {
            // 验证输入路径的有效性
            ValidatePaths(templatePath, outputDirectory);
            // 解析实体类文件
            var syntaxTree = ParseEntityClass(entityClassPath);
            // 获取实体类的属性信息
            var properties = GetEntityProperties(syntaxTree);
            // 加载模板文件
            var template = LoadTemplate(templatePath);
            // 渲染模板
            var result = RenderTemplate(template, syntaxTree, properties);
            // 写入输出文件
            WriteOutput(outputDirectory, syntaxTree, result);
        }

        // 验证路径是否存在
        private static void ValidatePaths(string templatePath, string outputDirectory)
        {
            // 检查模板文件是否存在
            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"未找到模板文件: {templatePath}");
            }

            // 检查输出目录是否存在，不存在则创建
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }
        }

        // 解析实体类文件并生成语法树
        private static SyntaxTree ParseEntityClass(string entityClassPath)
        {
            // 检查实体类文件是否存在
            if (!File.Exists(entityClassPath))
            {
                throw new FileNotFoundException($"未找到实体类文件: {entityClassPath}");
            }
            // 读取文件内容并解析为语法树
            var code = File.ReadAllText(entityClassPath);
            return CSharpSyntaxTree.ParseText(code);
        }

        // 从语法树中提取实体类的属性信息
        private static List<object> GetEntityProperties(SyntaxTree syntaxTree)
        {
            // 获取语法树的根节点
            var root = syntaxTree.GetRoot();
            // 查找第一个类声明
            var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

            // 如果找不到类声明则抛出异常
            if (classDeclaration == null)
            {
                throw new Exception($"未找到实体类");
            }

            // 提取属性信息并转换为对象列表
            return classDeclaration.Members
                .OfType<PropertyDeclarationSyntax>()
                .Select(p => (object)new
                {
                    Name = p.Identifier.Text,
                    Type = p.Type.ToString(),
                    Description = p.AttributeLists
                        .SelectMany(a => a.Attributes)
                        .FirstOrDefault(a => a.Name.ToString() == "Description")?
                        .ArgumentList?.Arguments.FirstOrDefault()?.Expression.ToString() ?? ""
                })
                .ToList();
        }

        // 加载模板文件
        private static Template LoadTemplate(string templatePath)
        {
            // 读取模板文件内容并解析为Template对象
            var templateContent = File.ReadAllText(templatePath);
            return Template.Parse(templateContent);
        }

        // 使用模板引擎渲染代码
        private static string RenderTemplate(Template template, SyntaxTree syntaxTree, List<object> properties)
        {
            // 获取语法树的根节点
            var root = syntaxTree.GetRoot();
            // 查找第一个类声明
            var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

            // 创建脚本对象并导入所需数据
            var scriptObject = new ScriptObject();
            scriptObject.Import(typeof(RoslynCodeHelper));
            scriptObject["EntityName"] = classDeclaration?.Identifier.Text;
            scriptObject["Description"] = classDeclaration?.AttributeLists
                        .SelectMany(a => a.Attributes)
                        .FirstOrDefault(a => a.Name.ToString() == "Description")?
                        .ArgumentList?.Arguments.FirstOrDefault()?.Expression.ToString() ?? "";
            scriptObject["Properties"] = properties;

            // 创建模板上下文并渲染模板
            var context = new TemplateContext();
            context.PushGlobal(scriptObject);

            return template.Render(context);
        }

        // 将生成的代码写入输出文件
        private static void WriteOutput(string outputDirectory, SyntaxTree syntaxTree, string result)
        {
            // 获取语法树的根节点
            var root = syntaxTree.GetRoot();
            // 查找第一个类声明
            var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
            // 构建输出文件路径并写入内容
            var outputPath = Path.Combine(outputDirectory, $"{classDeclaration?.Identifier.Text}Service.cs");
            File.WriteAllText(outputPath, result);
        }
    }
}
