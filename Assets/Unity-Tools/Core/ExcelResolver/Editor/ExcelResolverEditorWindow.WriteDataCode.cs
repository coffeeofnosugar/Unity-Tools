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
            CodeNamespace codeNamespace = new CodeNamespace(excelResolverConfig.GenerateDataClassNameSpace);
            compileUnit.Namespaces.Add(codeNamespace);
            
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
            
            CodeTypeDeclaration classType = new CodeTypeDeclaration(classCodeData.className)
            {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.Public,
                CustomAttributes = new CodeAttributeDeclarationCollection()
                {
                    new CodeAttributeDeclaration("Serializable")
                },
                BaseTypes = { new CodeTypeReference("IExcelData") }
            };
            codeNamespace.Types.Add(classType);
            
            CodeGeneratorOptions options = new CodeGeneratorOptions
            {
                BracingStyle = "C",
                BlankLinesBetweenMembers = false,
                VerbatimOrder = true,
            };

            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();
                provider.GenerateCodeFromCompileUnit(compileUnit, writer, options);
            }
        }
    }
}