using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.CSharp;

/* 使用CodeCompileUnit需要将Edit->Project Settings->Player->Other Settings->Api Compatibility Level改为.NET 4.x（或.NET Framework） */


namespace Tools.ExcelResolver.Editor
{
    public sealed partial class ExcelResolverEditorWindow
    {
        private void WriteDataCode(ClassCodeData classCodeData)
        {
            string outputPath = $"{excelResolverConfig.CodePathRoot}/{classCodeData.className}.cs";
            CodeCompileUnit compileUnit = new CodeCompileUnit();

            #region 命名空间

            CodeNamespace codeNamespace = new CodeNamespace(excelResolverConfig.GenerateDataClassNameSpace);
            compileUnit.Namespaces.Add(codeNamespace);
            
            #endregion

            #region 引用

            string[] classImports = new string[]
            {
                "System",
                "System.Collections",
                "System.Collections.Generic",
                "UnityEngine",
            };
            foreach (var import in classImports)
            {
                codeNamespace.Imports.Add(new CodeNamespaceImport(import));
            }
            
            #endregion

            #region 类

            CodeTypeDeclaration classType = new CodeTypeDeclaration(classCodeData.className)
            {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.Public,
                // CustomAttributes = new CodeAttributeDeclarationCollection()
                // {
                //     new CodeAttributeDeclaration("Serializable")
                // },
                BaseTypes =
                {
                    new CodeTypeReference("ScriptableObject"),
                    new CodeTypeReference("IExcelData")
                }
            };
            codeNamespace.Types.Add(classType);
            
            #endregion

            #region 字段

            foreach (var field in classCodeData.fields.Values)
            {
                CodeMemberField codeField = new CodeMemberField
                {
                    Attributes = MemberAttributes.Public,
                    Name = field.varName,
                    Type = new CodeTypeReference(field.type),
                    Comments =
                    {
                        new CodeCommentStatement("<summary>", true),
                        new CodeCommentStatement(field.info, true),
                    },
                };
                if (!string.IsNullOrEmpty(field.description)) 
                    codeField.Comments.Add(new CodeCommentStatement($"<c>{field.description}</c>", true));
                codeField.Comments.Add(new CodeCommentStatement("</summary>", true));
                classType.Members.Add(codeField);
            }
            
            #endregion

            #region 代码风格设置
            
            CodeGeneratorOptions options = new CodeGeneratorOptions
            {
                BracingStyle = "C",
                BlankLinesBetweenMembers = true,
                VerbatimOrder = true,
            };

            #endregion

            #region 写入文件
            
            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();
                provider.GenerateCodeFromCompileUnit(compileUnit, writer, options);
            }
            
            #endregion
        }
    }
}